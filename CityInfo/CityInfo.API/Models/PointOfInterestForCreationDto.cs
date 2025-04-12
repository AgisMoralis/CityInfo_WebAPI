using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointOfInterestForCreationDto
    {
        [Required]
        [MaxLength(10)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Description { get; set; } = null;
    }
}
