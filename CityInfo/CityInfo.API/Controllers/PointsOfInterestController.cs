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

        [HttpGet("{pointofinterestid}")]
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
    }
}
