namespace CityInfo.API.Models
{
    /// <summary>
    /// A city with its points of interests
    /// </summary>
    public class CityDto
    {
        /// <summary>
        /// The Id of the city
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the city
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The description of the city
        /// </summary>
        public string? Description { get; set; } = null;

        /// <summary>
        /// The total points of interests of the city
        /// </summary>
        public int NumberOfPointsOfInterest { get { return PointsOfInterest.Count; } }

        /// <summary>
        /// A list with all the points of interests of the city
        /// </summary>
        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();
    }
}
