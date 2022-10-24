using MovieCatalogAPI.Configurations;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;
using MovieCatalogAPI.Models.DTO;

namespace MovieCatalogAPI.Services
{
    public interface IMovieInfoService
    {
        List<MovieElementModel> GetMovieElements(int currentPage);       
    }

    public class MovieInfoService : IMovieInfoService
    {
        private MovieDBContext _dbContext;

        public MovieInfoService(MovieDBContext dbContext)
        {
            _dbContext = dbContext;
            PaginationData.TotalPageCount = (_dbContext.Movies.Count() + PaginationData.MaxItemsPerPage - 1) / PaginationData.MaxItemsPerPage;
        }

        

        public List<MovieElementModel> GetMovieElements(int currentPage)
        {
            return _dbContext.Movies.OrderBy(x => x.Id).Skip((currentPage - 1) * PaginationData.MaxItemsPerPage)
                .Take(PaginationData.MaxItemsPerPage)
                .Select(x => new MovieElementModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Poster = x.Poster,
                    Year = x.Year,
                    Country = x.Country,
                    Genres = x.Genres == null ? null : x.Genres.Select(g => new GenreModel { Id = g.Id, Name = g.Name }).ToList(),
                    Reviews = x.Reviews != null ? x.Reviews.Select(r => r.ToShort()).ToList() : null
                }).ToList();
        }
    }
}
