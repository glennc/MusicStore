using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Models
{
    public interface IAlbumRepository
    {
        Task<Album> GetAlbum(int albumId);
    }
}
