using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;
using System.Text.Json;

namespace MovieCatalogAPI.Services
{
    public interface IMovieDataService
    {
        Task<MovieElementModel> GetMovie(int page);
        void AddMovieToDB(MovieElementModel movie);
    }

    public class MovieDataService : IMovieDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MovieDBContext _dbContext;

        public MovieDataService(IHttpClientFactory httpClientFactory, MovieDBContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
        }

        public void AddMovieToDB(MovieElementModel movieDTO)
        {
            var movie = new Movie
            {
                Name = movieDTO.Name,
                Poster = movieDTO.Poster,
                Year = movieDTO.Year,
                Country = movieDTO.Country,
                Reviews = new List<Review>()
            };
            if (movieDTO.Genres != null)
            {
                movie.Genres = new List<Genre>();
                foreach (var genre in movieDTO.Genres)
                {
                    var genreInDB = _dbContext.Genres.FirstOrDefault(x => x.Id == genre.Id);
                    if (genreInDB != null)
                    {
                        movie.Genres.Add(genreInDB);
                        genreInDB.Movies?.Add(movie);
                    }
                    else
                    {
                        _dbContext.Genres.Add(new Genre
                        {
                            Id = genre.Id,
                            Name = genre.Name,
                            Movies = new List<Movie> { movie }
                        });
                    }
                }
            }
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
        }
                
        public async Task<MovieElementModel> GetMovie(int page)
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://react-midterm.kreosoft.space/api/movies/{1}");

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
            }
            throw new NotImplementedException();
        }
    }
}
