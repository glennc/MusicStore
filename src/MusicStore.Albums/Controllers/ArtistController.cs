using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicStore.Albums.Controllers
{
    [Route("api/[controller]")]
    public class ArtistController : Controller
    {
        private AlbumContext _context;

        public ArtistController(AlbumContext context)
        {
            _context = context;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<Artist> Get()
        {
            return _context.Artists.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Artist Get(int id)
        {
            return _context.Artists.Single(x => x.ArtistId == id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Artist value)
        {
            _context.Artists.Add(value);
            _context.SaveChanges();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Artist value)
        {
            _context.Artists.Update(value);
            _context.SaveChanges();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var artistToRemove = Get(id);
            _context.Artists.Remove(artistToRemove);
            _context.SaveChanges();
        }
    }
}
