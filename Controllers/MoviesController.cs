using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Data.ViewModels;
using MoviesApp.Models;

namespace MoviesApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MoviesAppContext _context;

        public MoviesController(MoviesAppContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var moviesAppContext = _context.Movie.Include(m => m.Cinema).Include(m => m.Producer);
            return View(await moviesAppContext.ToListAsync());
        }
        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            var allMovies = await _context.Movie.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                //var filteredResult = allMovies.Where(n => n.Name.ToLower().Contains(searchString.ToLower()) || n.Description.ToLower().Contains(searchString.ToLower())).ToList();

                var filteredResultNew = allMovies.Where(n => string.Equals(n.Name, searchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.Description, searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

                return View("Index", filteredResultNew);
            }

            return View("Index", allMovies);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Cinema)
                .Include(m => m.Producer)
                .Include(am=>am.Actor_Movie).ThenInclude(a=>a.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        public async Task<NewMovieDropdownsVM> GetNewMovieDropdownsValues()
        {
            var response = new NewMovieDropdownsVM()
            {
                Actors = await _context.Actor.OrderBy(n => n.FullName).ToListAsync(),
                Cinemas = await _context.Cinema.OrderBy(n => n.Name).ToListAsync(),
                Producers = await _context.Producer.OrderBy(n => n.FullName).ToListAsync()
            };

            return response;
        }
        // GET: Movies/Create
        public async Task<IActionResult> CreateAsync()
        {
            var movieDropdownsData = await GetNewMovieDropdownsValues();

            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewMovieVM movie) 
        {
            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

                return View(movie);
            }
            var newMovie = new Movie()
            {
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                ImageURL = movie.ImageURL,
                CinemaId = movie.CinemaId,
                StartDate = movie.StartDate,
                EndDate = movie.EndDate,
                MovieCategory = movie.MovieCategory,
                ProducerId = movie.ProducerId
            };
            await _context.Movie.AddAsync(newMovie);

            await _context.SaveChangesAsync();
            foreach (var actorId in movie.ActorId)
            {
                var newActorMovie = new Actor_Movie()
                {
                    MovieId = newMovie.Id,
                    ActorId = actorId
                };

                await _context.Actor_Movie.AddAsync(newActorMovie);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
         
            if (movie == null)
            {
                return NotFound();
            }
            var response = new NewMovieVM()
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                StartDate = movie.StartDate,
                EndDate = movie.EndDate,
                ImageURL = movie.ImageURL,
                MovieCategory = movie.MovieCategory,
                CinemaId = movie.CinemaId,
                ProducerId = movie.ProducerId,
               // ActorId = _context.Actor.Select 
            };

            var movieDropdownsData = await GetNewMovieDropdownsValues();
            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
            return View(response);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie)
        {
            if (id != movie.Id) return View("NotFound");

            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

                return View(movie);
            }

            var dbMovie = await _context.Movie.FirstOrDefaultAsync(n => n.Id == movie.Id);

            if (dbMovie != null)
            {
                dbMovie.Name = movie.Name;
                dbMovie.Description = movie.Description;
                dbMovie.Price = movie.Price;
                dbMovie.ImageURL = movie.ImageURL;
                dbMovie.CinemaId = movie.CinemaId;
                dbMovie.StartDate = movie.StartDate;
                dbMovie.EndDate = movie.EndDate;
                dbMovie.MovieCategory = movie.MovieCategory;
                dbMovie.ProducerId = movie.ProducerId;
                await _context.SaveChangesAsync();
            }

            //Remove existing actors
            var existingActorsDb = _context.Actor_Movie.Where(n => n.MovieId == movie.Id).ToList();
            _context.Actor_Movie.RemoveRange(existingActorsDb);
            await _context.SaveChangesAsync();

            //Add Movie Actors
            foreach (var actorId in movie.ActorId)
            {
                var newActorMovie = new Actor_Movie()
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                };
                await _context.Actor_Movie.AddAsync(newActorMovie);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Cinema)
                .Include(m => m.Producer)
                .Include(am => am.Actor_Movie).ThenInclude(a => a.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'MoviesAppContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);

            if (movie != null)
            {
                //List<Actor_Movie> Actors= new List<Actor_Movie>();
                //List <> Actors  = new SelectList(_context.Actor_Movie, "Id", "Name");
                var actors = await _context.Actor_Movie.ToListAsync();
                foreach (var item in actors)
                {
                    if (item.MovieId == movie.Id)
                    {
                        _context.Actor_Movie.Remove(item);
                    }
                }

                _context.Movie.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return _context.Movie.Any(e => e.Id == id);
        }
    }
}
