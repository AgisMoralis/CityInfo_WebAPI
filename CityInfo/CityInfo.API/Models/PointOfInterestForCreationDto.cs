using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    /// <summary>
    /// A point of interest of a city (used to create a new one)
    /// </summary>
    public class PointOfInterestForCreationDto
    {
        /// <summary>
        /// The name of the new point of interest
        /// </summary>
        [Required(ErrorMessage = "You should provide a name value for the new Point of Interest.")]
        [MaxLength(20, ErrorMessage = "The name value of the new Point of Interest shall not exceed the maximum length of 20 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The description of the new point of interest
        /// </summary>
        [MaxLength(25, ErrorMessage = "The description value of the new Point of Interest shall not exceed the maximum length of 25 characters.")]
        public string? Description { get; set; } = null;
    }
}
