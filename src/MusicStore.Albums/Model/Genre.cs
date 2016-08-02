﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.Albums
{
    public class Genre
    {
        public int GenreId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<Album> Albums { get; set; }
    }
}