using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointOfInterestForCreationDto
    {
        [Required(ErrorMessage = "You should provide a name value for the new Point of Interest.")]
        [MaxLength(10, ErrorMessage = "The name value of the new Point of Interest shall not exceed the maximum length of 10 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10, ErrorMessage = "The description value of the new Point of Interest shall not exceed the maximum length of 10 characters.")]
        public string? Description { get; set; } = null;
    }
}
