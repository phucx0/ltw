using DoAn.Models.Data;
using DoAn.Models.Movies;
using DoAn.Services;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieApiController : ControllerBase
    {
        private readonly MovieService _service;
        public MovieApiController(MovieService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<Movie?> GetMovieById(int id)
        {
            var movie = await _service.GetMovieById(id);
            return movie;
        }
    }
}
