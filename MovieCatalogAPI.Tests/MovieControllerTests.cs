using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieCatalogAPI.Controllers;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MovieCatalogAPI.Tests
{
    public class MovieControllerTests
    {
        private readonly IMovieInfoService? infoService;

        public MovieControllerTests()
        {
            var services = new ServiceCollection();
            var connection = "Server=localhost;Database=MovieDB;UserID=postgres;Password=2003;";
            services.AddTransient<IMovieConverterService,MovieConverterService>();
            services.AddDbContext<MovieDBContext>(opts => opts.UseNpgsql(connection));
            services.AddLogging();
            services.AddScoped<IMovieInfoService, MovieInfoService>();
            infoService = services.BuildServiceProvider().GetService<IMovieInfoService>();
        }

        [Fact]
        public void GetMoviesDataCheck()
        {
            //Arrange
            var controller = new MovieController(infoService);

            //Act
            var result = controller.Get(1);
            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MovieServiceNotNull()
        {

            Assert.NotNull(infoService);
        }
    }
}
