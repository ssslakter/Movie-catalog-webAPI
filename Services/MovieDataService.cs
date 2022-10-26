using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieCatalogAPI.Services
{
    public interface IMovieDataService
    {
        Task<MoviesPagedListModel> GetMovies(int page);
        Task AddMoviesPageToDB(int page);
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
        public async Task AddMoviesPageToDB(int page)
        {
            var model = await GetMovies(page);
            var movies = model.Movies;
            foreach (var movie in movies)
            {
                await AddMovieToDB(movie);
            }
        }

        private async Task AddMovieToDB(MovieElementModel movieDTO)
        {
            var movie = new Movie
            {
                Id = movieDTO.Id,
                Name = movieDTO.Name,
                Poster = movieDTO.Poster,
                Year = movieDTO.Year,
                Country = movieDTO.Country,
                Reviews = new List<Review>()
            };
            if (_dbContext.Movies.FirstOrDefault(x => x.Id == movie.Id) != null)
            {
                //throw exception
                return;
            }
            if (movieDTO.Genres != null)
            {
                movie.Genres = new List<Genre>();
                foreach (var genre in movieDTO.Genres)
                {
                    var genreInDB = _dbContext.Genres.FirstOrDefault(x => x.Id == genre.Id);
                    if (genreInDB != null)
                    {
                        movie.Genres.Add(genreInDB);
                        genreInDB.Movies.Add(movie);
                    }
                    else
                    {
                        var genreNew = new Genre
                        {
                            Id = genre.Id,
                            Name = genre.Name,
                            Movies = new List<Movie> { movie }
                        };
                        _dbContext.Genres.Add(genreNew);                       
                        movie.Genres.Add(genreNew);
                    }
                }
            }
            await AddMovieDetails(movie);
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
        }

        private async Task AddMovieDetails(Movie movie)
        {
            var details = await GetMovieDetails(movie.Id);
            movie.Time = details.Time;
            movie.Tagline = details.Tagline;
            movie.Description = details.Description;
            movie.Director = details.Director;
            movie.Budget = details.Budget;
            movie.Fees = details.Fees;
            movie.AgeLimit = details.AgeLimit;
        }

        public async Task<MoviesPagedListModel> GetMovies(int page)
        {
            return await GetRequestModel<MoviesPagedListModel>($"https://react-midterm.kreosoft.space/api/movies/{page}");
        }

        public async Task<MovieDetailsModel> GetMovieDetails(Guid id)
        {
            return await GetRequestModel<MovieDetailsModel>($"https://react-midterm.kreosoft.space/api/movies/details/{id}");
        }

        public async Task<T> GetRequestModel<T>(string requestString)
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                requestString);

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }; var json = await httpResponseMessage.Content.ReadAsStreamAsync();
                var model = await JsonSerializer.DeserializeAsync<T>(json, opts);
                return model;
            }
            throw new BadHttpRequestException("Bad request", 400);
        }
    }
}
