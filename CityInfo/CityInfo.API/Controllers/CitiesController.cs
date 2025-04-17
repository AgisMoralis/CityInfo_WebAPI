using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        // Private members
        private readonly ILogger<CitiesController> _logger;
        private readonly CitiesDatastore _citiesDatastore;

        public CitiesController(ILogger<CitiesController> logger, CitiesDatastore citiesDatastore)
        {
            _logger = logger;
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
            try
            {
                var cityToReturn = _citiesDatastore.Cities.FirstOrDefault(c => c.Id == id);
                if (cityToReturn is null)
                {
                    _logger.LogError($"The city with id {id} was not found in the Datastore, when trying to access the cities.");
                    return NotFound();
                }

                return Ok(cityToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to access the city with id {id}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
    }
}
