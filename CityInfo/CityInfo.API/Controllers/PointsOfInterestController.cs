using CityInfo.API.Models;
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

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = CitiesDatastore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city is null)
                {
                    _logger.LogInformation($"The city with id {cityId} was not found in the Datastore, when trying to access the points of interests.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex) 
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to access the points of interests of the city with id {cityId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDatastore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterestToReturn = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            return pointOfInterestToReturn is null ? NotFound() : Ok(pointOfInterestToReturn);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            // Because of the [ApiController] attribute, the annotations are automatically checked during model binding
            // that affects the ModelState dictionary. The [ApiController] attribute ensures, in case of an invalid ModelState,
            // that a bad request is returned as a response, with the validation error details in the body of the response
            // if (!ModelState.IsValid) 
            // {
            //     return BadRequest();
            // }

            var city = CitiesDatastore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }

            // Create a new Point of Interest and add it in the resources
            var maxPointOfInterestId = city.PointsOfInterest.Max(c => c.Id);
            var newPointOfInterest = new PointOfInterestDto()
            {
                Id = maxPointOfInterestId + 1,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };
            city.PointsOfInterest.Add(newPointOfInterest);

            // Returns a 201 HTTP response to successfully state that the new Point of Interest
            // was created in the resources. The body of the response includes that new Point of Interest.
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = newPointOfInterest.Id
                },
                newPointOfInterest);
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid, PointOfInterestForUpdatingDto pointOfInterest)
        {
            // NOTE: PUT actions can return a 200 response that includes the new Point of Interest as body of that response
            // NOTE: or can return a 204 response without any content in the results

            var city = CitiesDatastore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterestFromDatastore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
            if (pointOfInterestFromDatastore is null)
            {
                return NotFound();
            }

            pointOfInterestFromDatastore.Name = pointOfInterest.Name;
            pointOfInterestFromDatastore.Description = pointOfInterest.Description;

            // NOTE: Here we decided that our PUT action shall return a 204 response without any content
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdatingDto> patchDocument)
        {
            var city = CitiesDatastore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterestFromDatastore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
            if (pointOfInterestFromDatastore is null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdatingDto()
            {
                Name = pointOfInterestFromDatastore.Name,
                Description = pointOfInterestFromDatastore.Description
            };

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
            if(!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromDatastore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromDatastore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
        {
            var city = CitiesDatastore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterestFromDatastore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
            if (pointOfInterestFromDatastore is null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromDatastore);

            return NoContent();
        }
    }
}
