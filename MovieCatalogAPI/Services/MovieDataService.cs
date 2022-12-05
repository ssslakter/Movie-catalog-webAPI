using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MovieCatalogAPI.Exceptions;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Models.Core_data;
using MovieCatalogAPI.Models.DTO;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace MovieCatalogAPI.Services
{
    public interface IMovieDataService
    {
        Task DeleteMovie(Guid movieId);
        Task AddMovie(MovieInsertModel movieInsertModel);
        Task EditMovie(Guid movieId, MovieInsertModel movieInsertModel);
        Task<MoviesPagedListModel> GetMovies(int page);
        Task AddMoviesPageToDB(int page);
        IEnumerable<GenreModel> GetGenres();

    }

    public class MovieDataService : IMovieDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MovieDBContext _dbContext;
        private readonly ILogger<MovieDataService> _logger;

        public MovieDataService(IHttpClientFactory httpClientFactory, MovieDBContext dbContext, ILogger<MovieDataService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
            _logger = logger;
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
                    var genreInDB = await _dbContext.Genres.Include(x => x.Movies).FirstOrDefaultAsync(x => x.Id == genre.Id);
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
            await _dbContext.SaveChangesAsync();
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

        async Task IMovieDataService.AddMovie(MovieInsertModel movieInsertModel)
        {
            //var genres = await GetGenresInDB(movieInsertModel.Genres);            
            var movie = new Movie
            {
                Name = movieInsertModel.Name,
                Poster = movieInsertModel.Poster,
                Year = movieInsertModel.Year,
                Country = movieInsertModel.Country,
                Time = movieInsertModel.Time,
                Tagline = movieInsertModel.Tagline,
                Description = movieInsertModel.Description,
                Director = movieInsertModel.Director,
                Budget = movieInsertModel.Budget,
                Fees = movieInsertModel.Fees,
                AgeLimit = movieInsertModel.AgeLimit,
                Reviews = new List<Review>(),
                Genres = new List<Genre>()
            };
            foreach (var genre in movieInsertModel.Genres)
            {
                var genreInDB = await _dbContext.Genres.Include(x => x.Movies).FirstOrDefaultAsync(x => x.Id == genre.Id);
                if (genreInDB != null)
                {
                    movie.Genres.Add(genreInDB);
                    genreInDB.Movies.Add(movie);
                }
            }
            _dbContext.Movies.Add(movie);
            await _dbContext.SaveChangesAsync();
        }

        //private async Task<ICollection<Genre>> GetGenresInDB(IEnumerable<string> genreNames)
        //{
        //    var genres = new List<Genre>();
        //    foreach (var genreName in genreNames)
        //    {
        //        var genre = await _dbContext.Genres.Include(x => x.Movies).FirstOrDefaultAsync(x => x.Name == genreName);
        //        if (genre != null)
        //        {
        //            genres.Add(genre);
        //        }
        //    }
        //    return genres;
        //}

        public async Task DeleteMovie(Guid movieId)
        {
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.Log(LogLevel.Information, $"Not found movie with id {movieId}");
                throw new NotFoundException($"Not found movie with id {movieId}");
            }
            _dbContext.Remove(movie);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditMovie(Guid movieId, MovieInsertModel movieInsertModel)
        {
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.Log(LogLevel.Information, $"Not found movie with id {movieId}");
                throw new NotFoundException($"Not found movie with id {movieId}");
            }
            movie.Name = movieInsertModel.Name;
            movie.Poster = movieInsertModel.Poster;
            movie.Year = movieInsertModel.Year;
            movie.Country = movieInsertModel.Country;
            movie.Time = movieInsertModel.Time;
            movie.Tagline = movieInsertModel.Tagline;
            movie.Description = movieInsertModel.Description;
            movie.Director = movieInsertModel.Director;
            movie.Budget = movieInsertModel.Budget;
            movie.Fees = movieInsertModel.Fees;
            movie.AgeLimit = movieInsertModel.AgeLimit;
            await _dbContext.SaveChangesAsync();
        }
        public IEnumerable<GenreModel> GetGenres()
        {
            return _dbContext.Genres.Select(x => new GenreModel { Id = x.Id, Name = x.Name });
        }
    }
}
