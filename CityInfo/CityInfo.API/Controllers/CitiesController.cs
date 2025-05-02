using AutoMapper;
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
        private readonly ILogger<CitiesController> _logger;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ILogger<CitiesController> logger, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger;
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery(Name = "namefilter")] string? name, string? searchQuery)
        {
            // The parameter "[FromQuery(Name = "name")] string? name" is used as an explicit indicator how an HTTP query/filter can be used in this action.
            // The "namefilter" shall be the parameter used in the HTTP query and it's value is assigned to the input 'name' variable.
            // The "[FromQuery(Name = "name")] string? name" can be defined simply as "string? name" and the HTTP query/filter would autmatically match "name" filter if used in the HTTP query
            try
            {
                var cityEntities = await _cityInfoRepository.GetCitiesAsync(name, searchQuery);
                var result = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
                return Ok(result);
            }
            catch (Exception ex) 
            {
                _logger.LogCritical("Unexpected exception occurred when trying to get all cities from the database.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            try
            {
                var cityEntity = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
                if (cityEntity is null)
                {
                    _logger.LogError($"The city with id {id} was not found in the Datastore, when trying to access the cities.");
                    return NotFound();
                }

                if (includePointsOfInterest) 
                {
                    return Ok(_mapper.Map<CityDto>(cityEntity));
                }
                return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityEntity));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to access the city with id {id}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
    }
}
