using Microsoft.EntityFrameworkCore;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Cart
{
    public class CartContext : DbContext
    {
        public CartContext(DbContextOptions<CartContext> options)
            :base(options)
        {
        }

        public DbSet<CartItem> CartItems { get; set; }
    }
}
