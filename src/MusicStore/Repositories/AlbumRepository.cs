using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MusicStore.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicStore.Models
{
    public class AlbumRepository : IAlbumRepository
    {
        private AppSettings _appSettings;
        private IMemoryCache _cache;
        private HttpClient _httpClient;
        private ILogger<StoreController> _logger;

        public AlbumRepository(IOptions<AppSettings> options, ILogger<StoreController> logger, IMemoryCache cache)
        {
            _appSettings = options.Value;
            _logger = logger;
            _cache = cache;
            _httpClient = new HttpClient();
        }
        // GET: /<controller>/
        public async Task<Album> GetAlbum(int id)
        {
            _logger.LogInformation($"RetrievingAlbums from: {_appSettings.AlbumsUrl}/album/{id}");
            //TODO: Move caching to the album API...
            var cacheKey = string.Format("album_{0}", id);
            Album album = null;
            if (!_cache.TryGetValue(cacheKey, out album))
            {
                var albumString = await _httpClient.GetStringAsync($"{_appSettings.AlbumsUrl}/album/{id}");

                if (albumString != null)
                {
                    album = JsonConvert.DeserializeObject<Album>(albumString);
                }

                if (album != null)
                {
                    if (_appSettings.CacheDbResults)
                    {
                        //Remove it from cache if not retrieved in last 10 minutes
                        _cache.Set(
                            cacheKey,
                            album,
                            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    }
                }
            }
            return album;
        }
    }
}
