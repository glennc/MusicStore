﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicStore.Albums.Controllers
{
    [Route("[controller]")]
    public class GenreController : Controller
    {
        private AlbumContext _context;

        public GenreController(AlbumContext context)
        {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<Genre> Get()
        {
            var genres = _context.Genres
                           .AsNoTracking()
                           .ToList();
            return genres;
        }

        [HttpGet("{name}")]
        public Genre Get(string name)
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            return _context.Genres
                .Include(g => g.Albums)
                .Where(g => g.Name == name)
                .FirstOrDefault();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Genre value)
        {
            _context.Genres.Add(value);
            _context.SaveChanges();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Genre value)
        {
            _context.Genres.Update(value);
            _context.SaveChanges();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var genreToRemove = _context.Genres.Single(x => x.GenreId == id);
            _context.Genres.Remove(genreToRemove);
            _context.SaveChanges();
        }
    }
}
