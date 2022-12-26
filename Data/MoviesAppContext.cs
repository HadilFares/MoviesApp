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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor_Movie>().HasKey(am => new
            {
                am.ActorId,
                am.MovieId
            });

            modelBuilder.Entity<Actor_Movie>().HasOne(m => m.Movie).WithMany(am => am.Actor_Movie).HasForeignKey(m => m.MovieId);
            modelBuilder.Entity<Actor_Movie>().HasOne(m => m.Actor).WithMany(am => am.Actor_Movie).HasForeignKey(m => m.ActorId);


            base.OnModelCreating(modelBuilder);
        }

        public DbSet<MoviesApp.Models.Actor> Actor { get; set; } = default!;

        public DbSet<MoviesApp.Models.Movie> Movie { get; set; }

        public DbSet<MoviesApp.Models.Cinema> Cinema { get; set; }

        public DbSet<MoviesApp.Models.Producer> Producer { get; set; }

        public DbSet<MoviesApp.Models.Actor_Movie> Actor_Movie { get; set; }
      
    }
}
