using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Watchlist.Data;
using Watchlist.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Watchlist.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WatchlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() =>
        _userManager.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<string> GetCurrentUserId()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            return user?.Id;
        }

        [HttpGet]
        public async void Watched(int id, Boolean val)
        {
            var userId = await GetCurrentUserId();
            var userMovies = _context.UserMovies.FirstOrDefault(x =>
                    x.MovieId == id && x.UserId == userId);
            if (userMovies != null)
            {
                userMovies.Watched = !val;
            }

            // now we can save the changes to the database
            await _context.SaveChangesAsync();
            // and our return value (-1, 0, or 1) back to the script that called
            // this method from the Index page
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var id = await GetCurrentUserId();
            var userMovies = _context.UserMovies.Where(x => x.UserId == id);
            var model = userMovies.Select(x => new MovieViewModel
            {
                MovieId = x.MovieId,
                Title = x.Movie.Title,
                Year = x.Movie.Year,
                Watched = x.Watched,
                InWatchlist = true,
                Rating = x.Rating
            }).ToList();

            return View(model);
        }
    }
}
