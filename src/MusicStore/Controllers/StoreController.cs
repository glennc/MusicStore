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
        private IAlbumRepository _albumRepository;

        public StoreController(IOptions<AppSettings> options, ILogger<StoreController> logger, IAlbumRepository albumRepository)
        {
            _appSettings = options.Value;
            _logger = logger;
            _albumRepository = albumRepository;
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

        public async Task<IActionResult> Details(int id)
        {
            var album = await _albumRepository.GetAlbum(id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }
    }
}