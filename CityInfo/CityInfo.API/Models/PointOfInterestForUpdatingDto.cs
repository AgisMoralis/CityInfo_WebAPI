using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    /// <summary>
    /// A point of interest of a city (used to update an existing one)
    /// </summary>
    public class PointOfInterestForUpdatingDto
    {
        /// <summary>
        /// The updated name of an existing point of interest
        /// </summary>
        [Required(ErrorMessage = "You should provide a name value for the updated Point of Interest.")]
        [MaxLength(20, ErrorMessage = "The name value of the new Point of Interest shall not exceed the maximum length of 20 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The updated description of an existing point of interest
        /// </summary>
        [MaxLength(25, ErrorMessage = "The description value of the updated Point of Interest shall not exceed the maximum length of 25 characters.")]
        public string? Description { get; set; } = null;
    }
}
