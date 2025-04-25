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
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
            var result = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            return Ok();
            //try
            //{
            //    var cityToReturn = _citiesDatastore.Cities.FirstOrDefault(c => c.Id == id);
            //    if (cityToReturn is null)
            //    {
            //        _logger.LogError($"The city with id {id} was not found in the Datastore, when trying to access the cities.");
            //        return NotFound();
            //    }
            //
            //    return Ok(cityToReturn);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogCritical($"Unexpected exception occurred when trying to access the city with id {id}.", ex);
            //    return StatusCode(500, $"A problem occurred while handling your request.");
            //}
        }
    }
}
