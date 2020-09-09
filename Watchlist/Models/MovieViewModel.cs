using System;
namespace Watchlist.Models
{
    public class MovieViewModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public bool InWatchlist { get; set; }
        public bool Watched { get; set; }
        public int? Rating { get; set; }
    }
}
