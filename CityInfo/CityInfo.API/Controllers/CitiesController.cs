using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class CitiesController : ControllerBase
    {
        // Private members
        private readonly ILogger<CitiesController> _logger;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        private const int maxCitiesPageSize = 20;

        public CitiesController(ILogger<CitiesController> logger, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger;
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get all cities without their points of interests
        /// </summary>
        /// <param name="name">The filter that shall be applied in the cities returned, based on their name (optional)</param>
        /// <param name="searchQuery">The query that shall be applied in the cities filtered, checking if their name includes that keyword (optional)</param>
        /// <param name="pageNumber">The specified page to show in the results, after returning those applicable filtered and queried results</param>
        /// <param name="pageSize">The amount of cities that each page can include in the results</param>
        /// <returns>All cities without their points of interests</returns>
        /// <response code="200">Returns the list with all filtered cities</response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
            [FromQuery(Name = "namefilter")] string? name,
            string? searchQuery,
            int pageNumber = 1,
            int pageSize = 10)
        {
            // The parameter "[FromQuery(Name = "name")] string? name" is used as an explicit indicator how an HTTP query/filter can be used in this action.
            // The "namefilter" shall be the parameter used in the HTTP query and it's value is assigned to the input 'name' variable.
            // The "[FromQuery(Name = "name")] string? name" can be defined simply as "string? name" and the HTTP query/filter would autmatically match "name" filter if used in the HTTP query
            try
            {
                pageSize = pageSize > maxCitiesPageSize ? maxCitiesPageSize : pageSize;
                var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

                // Add an new extra header in the HTTP response that will include the pagination metadata serialized in JSON format
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

                // Return the response that includes the result
                var result = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unexpected exception occurred when trying to get all cities from the database.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }

        /// <summary>
        /// Get a city by an Id, with or without its points of interests
        /// </summary>
        /// <param name="cityId">The Id of the city to get</param>
        /// <param name="includePointsOfInterest">Whether or not to include the points of interests of the city returned</param>
        /// <returns>A city with or without its points of interests</returns>
        /// <response code="200">Returns the requested city</response>
        [HttpGet("{cityId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCity(int cityId, bool includePointsOfInterest = false)
        {
            try
            {
                var cityEntity = await _cityInfoRepository.GetCityAsync(cityId, includePointsOfInterest);
                if (cityEntity is null)
                {
                    _logger.LogError($"The city with id {cityId} was not found in the Datastore, when trying to access the cities.");
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
                _logger.LogCritical($"Unexpected exception occurred when trying to access the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
    }
}
