using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        // Private members
        private readonly CitiesDatastore _citiesDatastore;

        public CitiesController(CitiesDatastore citiesDatastore)
        {
            _citiesDatastore = citiesDatastore ?? throw new ArgumentNullException(nameof(citiesDatastore));
        }

        [HttpGet()]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_citiesDatastore.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var cityToReturn = _citiesDatastore.Cities.FirstOrDefault(c => c.Id == id);
            return cityToReturn is null ? NotFound() : Ok(cityToReturn);
        }
    }
}
