using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/[controller]")]
    public class PointsOfInterestController : ControllerBase
    {
        // Private members
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"The city with id {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                var pointsOfInterestOfCity = await _cityInfoRepository.GetPointsOfInterestOfCityAsync(cityId);
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestOfCity));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to access the points of interests of the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"The city with id {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                var pointOfInterestToReturn = await _cityInfoRepository.GetPointOfInterestOfCityAsync(cityId, pointOfInterestId);
                if (pointOfInterestToReturn is null) 
                {
                    _logger.LogInformation($"The point of interest with id {pointOfInterestId} from the city with ID {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestToReturn));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to access the points of interests of the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            // Because of the [ApiController] attribute, the annotations are automatically checked during model binding
            // that affects the ModelState dictionary. The [ApiController] attribute ensures, in case of an invalid ModelState,
            // that a bad request is returned as a response, with the validation error details in the body of the response
            // if (!ModelState.IsValid) 
            // {
            //     return BadRequest();
            // }

            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"The city with id {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                // Map the new Point of Interest to the entity and add it in the resources
                var finalPointOfInterestEntityToSave = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);
                await _cityInfoRepository.AddPointOfInterestOnCityAsync(cityId, finalPointOfInterestEntityToSave);
                await _cityInfoRepository.SaveChangesAsync();

                // Returns a 201 HTTP response to successfully state that the new Point of Interest
                // was created in the resources. The body of the response includes that new Point of Interest.
                var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterestEntityToSave);
                return CreatedAtRoute("GetPointOfInterest",
                    new
                    {
                        cityId = cityId,
                        pointOfInterestId = createdPointOfInterestToReturn.Id
                    },
                    createdPointOfInterestToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to create a new point of interests for the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
        
        //[HttpPut("{pointofinterestid}")]
        //public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid, PointOfInterestForUpdatingDto pointOfInterest)
        //{
        //    // NOTE: PUT actions can return a 200 response that includes the new Point of Interest as body of that response
        //    // NOTE: or can return a 204 response without any content in the results
        //
        //    var city = _citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city is null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var pointOfInterestFromDatastore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
        //    if (pointOfInterestFromDatastore is null)
        //    {
        //        return NotFound();
        //    }
        //
        //    pointOfInterestFromDatastore.Name = pointOfInterest.Name;
        //    pointOfInterestFromDatastore.Description = pointOfInterest.Description;
        //
        //    // NOTE: Here we decided that our PUT action shall return a 204 response without any content
        //    return NoContent();
        //}
        //
        //[HttpPatch("{pointofinterestid}")]
        //public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdatingDto> patchDocument)
        //{
        //    var city = _citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city is null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var pointOfInterestFromDatastore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
        //    if (pointOfInterestFromDatastore is null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var pointOfInterestToPatch = new PointOfInterestForUpdatingDto()
        //    {
        //        Name = pointOfInterestFromDatastore.Name,
        //        Description = pointOfInterestFromDatastore.Description
        //    };
        //
        //    // The 'ModelState' will report as invalid if there are mistakes in the input model
        //    // of this POST action: "JsonPatchDocument<PointOfInterestForUpdatingDto> patchDocument"
        //    // If that input JsonPatchDocument is valid and can be used for patching a
        //    // "PointOfInterest" object, the 'ModelState' will report itself as valid
        //    patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //
        //    // Check if the "PointOfInterestForUpdatingDto" is still valid after applying the JsonPatchDocument
        //    // This will use the attributes of the DTO object (model) for the validation
        //    if (!TryValidateModel(pointOfInterestToPatch))
        //    {
        //        return BadRequest(ModelState);
        //    }
        //
        //    pointOfInterestFromDatastore.Name = pointOfInterestToPatch.Name;
        //    pointOfInterestFromDatastore.Description = pointOfInterestToPatch.Description;
        //
        //    return NoContent();
        //}
        //
        //[HttpDelete("{pointofinterestid}")]
        //public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
        //{
        //    var city = _citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city is null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var pointOfInterestFromDatastore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
        //    if (pointOfInterestFromDatastore is null)
        //    {
        //        return NotFound();
        //    }
        //
        //    _ = city.PointsOfInterest.Remove(pointOfInterestFromDatastore);
        //
        //    _mailService.Send("Point of interest deleted.", $"The Point of Interest with ID {pointofinterestid} was deleted from the city with ID {cityId}.");
        //
        //    return NoContent();
        //}
    }
}
