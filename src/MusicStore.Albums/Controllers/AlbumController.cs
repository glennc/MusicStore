using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicStore.Albums.Controllers
{
    [Route("[controller]")]
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
            return _context.Albums
                            .AsNoTracking()
                           .Include(a => a.Artist)
                           .Include(a => a.Genre)
                           .ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Album Get(int id)
        {
            var album =  _context.Albums
                        .AsNoTracking()
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .SingleOrDefault(x => x.AlbumId == id);
            //TODO: Work around circular serialization. I want to find out if we can avoid it without resorting to a DTO.
            album.Genre.Albums.Clear();
            return album;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Album album)
        {
            _context.Albums.Add(album);
            _context.SaveChanges();
        }

        // PUT api/values/5
        //TODO: This implementation isn't really right. Will need to fix it.
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Album album)
        {
            _context.Albums.Update(album);
            _context.SaveChanges();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var albumToRemove = Get(id);
            _context.Albums.Remove(albumToRemove);
            _context.SaveChanges();
        }
    }
}
