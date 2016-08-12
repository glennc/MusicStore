using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using MusicStore.ViewModels;
using Newtonsoft.Json;

namespace MusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;

        private AppSettings _appSettings;

        private HttpClient _httpClient;
        private IAlbumRepository _albumRepository;

        public ShoppingCartController(MusicStoreContext dbContext, ILogger<ShoppingCartController> logger, IOptions<AppSettings> options, IAlbumRepository albumRepo)
        {
            DbContext = dbContext;
            _logger = logger;
            _appSettings = options.Value;
            _httpClient = new HttpClient();
            _albumRepository = albumRepo;
        }

        public MusicStoreContext DbContext { get; }

        //
        // GET: /ShoppingCart/
        public async Task<IActionResult> Index()
        {
            var cartItems = await _httpClient.GetStringAsync($"{_appSettings.CartUrl}/{GetCartId()}");

            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartItems);

            //Fetch album data from the album service.
            cart.ForEach(async x =>
            {
                x.Album = await _albumRepository.GetAlbum(x.AlbumId);
            });

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart,
                CartTotal = cart.Select(x=>x.Album.Price * x.Count).Sum()
            };

            // Return the view
            return View(viewModel);
        }

        private string GetCartId()
        {
            var cartId = HttpContext.Session.GetString("Session");

            if (cartId == null)
            {
                //A GUID to hold the cartId. 
                cartId = Guid.NewGuid().ToString();

                // Send cart Id as a cookie to the client.
                HttpContext.Session.SetString("Session", cartId);
            }

            return cartId;
        }

        //
        // GET: /ShoppingCart/AddToCart/5

        public async Task<IActionResult> AddToCart(int id, CancellationToken requestAborted)
        {
            // Retrieve the album from the database
            var addedAlbum = await DbContext.Albums
                .SingleAsync(album => album.AlbumId == id);

            var result = await _httpClient.PutAsync($"{_appSettings.CartUrl}/{GetCartId()}/{id}", null);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(DbContext, HttpContext);

            _logger.LogInformation("Album {albumId} was added to the cart.", addedAlbum.AlbumId);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(
            int id,
            CancellationToken requestAborted)
        {
            // Get the name of the album to display confirmation
            //TODO: This whole method feels bad... should at least get rid of the need for the album title. Then think about price data.
            var cartString = await _httpClient.GetStringAsync($"{_appSettings.CartUrl}/{GetCartId()}");
            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartString);
            var cartItem = cart.SingleOrDefault(x => x.CartItemId == id);

            cart.ForEach(async x =>
            {
                x.Album = await _albumRepository.GetAlbum(x.AlbumId);
            });

            string message;
            int itemCount;
            if (cartItem != null)
            {
                var result = await _httpClient.DeleteAsync($"{_appSettings.CartUrl}/{GetCartId()}/{id}");
                // Remove from cart
                // TODO: This seems dumb...
                itemCount = int.Parse(await result.Content.ReadAsStringAsync());
                cartItem.Count = itemCount;
                string removed = (itemCount > 0) ? " 1 copy of " : string.Empty;
                message = removed + cartItem.Album.Title + " has been removed from your shopping cart.";
            }
            else
            {
                itemCount = 0;
                message = "Could not find this item, nothing has been removed from your shopping cart.";
            }

            var results = new ShoppingCartRemoveViewModel
            {
                Message = message,
                CartTotal = cart.Select(x=>x.Album.Price * x.Count).Sum(),
                CartCount = cart.Count(),
                ItemCount = itemCount,
                DeleteId = id
            };

            _logger.LogInformation("Album {id} was removed from a cart.", id);

            return Json(results);
        }
    }
}