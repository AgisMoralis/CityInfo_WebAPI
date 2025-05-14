namespace CityInfo.API.Models
{
    /// <summary>
    /// A point of interest of a city
    /// </summary>
    public class PointOfInterestDto
    {
        /// <summary>
        /// The Id of the point of interest
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the point of interest
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The description of the point of interest
        /// </summary>
        public string? Description { get; set; } = null;
    }
}
