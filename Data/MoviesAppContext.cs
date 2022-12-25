using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Models;

namespace MoviesApp.Data
{
    public class MoviesAppContext : DbContext
    {
        public MoviesAppContext (DbContextOptions<MoviesAppContext> options)
            : base(options)
        {
        }

        public DbSet<MoviesApp.Models.Actor> Actor { get; set; } = default!;

        public DbSet<MoviesApp.Models.Movie> Movie { get; set; }

        public DbSet<MoviesApp.Models.Cinema> Cinema { get; set; }

        public DbSet<MoviesApp.Models.Producer> Producer { get; set; }

        public DbSet<MoviesApp.Models.Actor_Movie> Actor_Movie { get; set; }
        public object Producers { get; internal set; }
    }
}
