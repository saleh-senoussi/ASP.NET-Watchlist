using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Watchlist.Data;
using Watchlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Watchlist.Controllers
{
    //[Authorize]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Current User
        [HttpGet]
        public async Task<string> GetCurrentUserId()
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            return usr?.Id;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() =>
        _userManager.GetUserAsync(HttpContext.User);


        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var userId = await GetCurrentUserId();
            var model = await _context.Movies.Select(x =>
                new MovieViewModel
                {
                    MovieId = x.Id,
                    Title = x.Title,
                    Year = x.Year
                }).ToListAsync();
            foreach (var item in model)
            {
                var m = await _context.UserMovies.FirstOrDefaultAsync(x =>
                    x.UserId == userId && x.MovieId == item.MovieId);
                if (m != null)
                {
                    item.InWatchlist = true;
                    item.Rating = m.Rating;
                    item.Watched = m.Watched;
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> AddRemove(int id, int val)
        {
            int retval = -1;
            var userId = await GetCurrentUserId();
            if (val == 1)
            {
                // if a record exists in UserMovies that contains both the user’s
                // and movie’s Ids, then the movie is in the watchlist and can
                // be removed
                var movie = _context.UserMovies.FirstOrDefault(x =>
                    x.MovieId == id && x.UserId == userId);
                if (movie != null)
                {
                    _context.UserMovies.Remove(movie);
                    retval = 0;
                }

            }
            else
            {
                // the movie is not currently in the watchlist, so we need to
                // build a new UserMovie object and add it to the database
                _context.UserMovies.Add(
                    new UserMovie
                    {
                        UserId = userId,
                        MovieId = id,
                        Watched = false,
                        Rating = 0
                    }
                );
                retval = 1;
            }
            // now we can save the changes to the database
            await _context.SaveChangesAsync();
            // and our return value (-1, 0, or 1) back to the script that called
            // this method from the Index page
            return Json(retval);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
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
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
