using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient = new HttpClient();

        private readonly ILogger _logger;

        public StoreController(IOptions<AppSettings> options, ILogger<StoreController> logger)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        //
        // GET: /Store/
        public async Task<IActionResult> Index()
        {
            var genreString = await _httpClient.GetStringAsync($"{_appSettings.AlbumsUrl}/genre");
            var genres = JsonConvert.DeserializeObject<IEnumerable<Genre>>(genreString);

            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco
        public async Task<IActionResult> Browse(string genre)
        {
            var genreString = await _httpClient.GetStringAsync($"{_appSettings.AlbumsUrl}/genre/{genre}");

            if (string.IsNullOrEmpty(genreString))
            {
                return NotFound();
            }

            var genreModel = JsonConvert.DeserializeObject<Genre>(genreString);

            return View(genreModel);
        }

        public async Task<IActionResult> Details(
            [FromServices] IMemoryCache cache,
            int id)
        {
            _logger.LogInformation($"RetrievingAlbums from: {_appSettings.AlbumsUrl}/album/{id}");
            //TODO: Move caching to the album API...
            var cacheKey = string.Format("album_{0}", id);
            Album album = null;
            if (!cache.TryGetValue(cacheKey, out album))
            {
                var albumString = await _httpClient.GetStringAsync($"{_appSettings.AlbumsUrl}/album/{id}");

                if(albumString!= null)
                {
                    album = JsonConvert.DeserializeObject<Album>(albumString);
                }

                if (album != null)
                {
                    if (_appSettings.CacheDbResults)
                    {
                        //Remove it from cache if not retrieved in last 10 minutes
                        cache.Set(
                            cacheKey,
                            album,
                            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    }
                }
            }

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }
    }
}