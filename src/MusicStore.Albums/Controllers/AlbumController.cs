using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Albums.Controllers
{
    [Route("api/[controller]")]
    public class AlbumController : Controller
    {
        private AlbumContext _context;

        public AlbumController(AlbumContext context)
        {
            _context = context;
        }
        
        // GET api/values
        [HttpGet]
        public IEnumerable<Album> Get()
        {
            return _context.Albums.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Album Get(int id)
        {
            //Async?
            return  _context.Albums.SingleOrDefault(x => x.AlbumId == id);
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]Album album)
        {
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]Album album)
        {
            _context.Albums.Update(album);
            await _context.SaveChangesAsync();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var albumToRemove = Get(id);
            _context.Albums.Remove(albumToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
