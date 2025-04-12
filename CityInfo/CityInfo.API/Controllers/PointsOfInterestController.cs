using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/[controller]")]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet()]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDatastore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            return city is null ? NotFound() : Ok(city.PointsOfInterest);
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

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = newPointOfInterest.Id
                },
                newPointOfInterest);
        }
    }
}
