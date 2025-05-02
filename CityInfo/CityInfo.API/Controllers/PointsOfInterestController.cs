using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
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
        
        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointofinterestid, PointOfInterestForUpdatingDto pointOfInterest)
        {
            try
            {
                // NOTE: PUT actions can return a 200 response that includes the new Point of Interest as body of that response
                // NOTE: or can return a 204 response without any content in the results

                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"The city with id {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestOfCityAsync(cityId, pointofinterestid);
                if (pointOfInterestEntity is null)
                {
                    _logger.LogInformation($"The point of interest with id {pointofinterestid} from the city with ID {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                _mapper.Map(pointOfInterest, pointOfInterestEntity);
                await _cityInfoRepository.SaveChangesAsync();

                // NOTE: Here we decided that our PUT action shall return a 204 response without any content
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to update the point of interest with id {pointofinterestid} for the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
        
        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid,
            JsonPatchDocument<PointOfInterestForUpdatingDto> patchDocument)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"The city with id {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestOfCityAsync(cityId, pointofinterestid);
                if (pointOfInterestEntity is null)
                {
                    _logger.LogInformation($"The point of interest with id {pointofinterestid} from the city with ID {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdatingDto>(pointOfInterestEntity);

                // The 'ModelState' will report as invalid if there are mistakes in the input model
                // of this POST action: "JsonPatchDocument<PointOfInterestForUpdatingDto> patchDocument"
                // If that input JsonPatchDocument is valid and can be used for patching a
                // "PointOfInterest" object, the 'ModelState' will report itself as valid
                patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the "PointOfInterestForUpdatingDto" is still valid after applying the JsonPatchDocument
                // This will use the attributes of the DTO object (model) for the validation
                if (!TryValidateModel(pointOfInterestToPatch))
                {
                    return BadRequest(ModelState);
                }

                _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
                await _cityInfoRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to partially update the point of interest with id {pointofinterestid} for the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
        
        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointofinterestid)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"The city with id {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestOfCityAsync(cityId, pointofinterestid);
                if (pointOfInterestEntity is null)
                {
                    _logger.LogInformation($"The point of interest with id {pointofinterestid} from the city with ID {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
                await _cityInfoRepository.SaveChangesAsync();

                _mailService.Send("Point of interest deleted.",
                    $"The Point of Interest with ID {pointOfInterestEntity.Id} was deleted from the city with ID {cityId}.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to delete the point of interest with id {pointofinterestid} for the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
    }
}
