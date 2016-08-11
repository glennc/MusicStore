using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Cart
{
    //TODO: Need to go over this and work on response codes, error conditions, etc. Make this more RESTful.
    [Route("/")]
    public class CartController : Controller
    {
        CartContext _cartContext;

        public CartController (CartContext cartContext)
        {
            _cartContext = cartContext;
        }

        [HttpGet("/{id}")]
        public IEnumerable<CartItem> Get(string id)
        {
            var cartItems = _cartContext.CartItems.Include(c=>c.Album).Where(x=>x.CartId == id);

            return cartItems;
        }

        [HttpPut("/{id}/{albumId}")]
        public IActionResult AddAlbum(string id, int albumId)
        {
            // Get the matching cart and album instances
            var cartItem = _cartContext.CartItems.SingleOrDefault(
                c => c.CartId == id
                && c.AlbumId == albumId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new CartItem
                {
                    AlbumId = albumId,
                    CartId = id,
                    Count = 1,
                    DateCreated = DateTime.Now
                };

                _cartContext.CartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                cartItem.Count++;
            }

            _cartContext.SaveChanges();
            return Ok();
        }

        [HttpDelete("/{id}/{cartItemId}")]
        public int DeleteAlbum(string id, int cartItemId)
        {
            // Get the cart
            var cartItem = _cartContext.CartItems.SingleOrDefault(
                cart => cart.CartId == id
                && cart.CartItemId == cartItemId);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _cartContext.CartItems.Remove(cartItem);
                }
            }

            return itemCount;
        }
    }
}