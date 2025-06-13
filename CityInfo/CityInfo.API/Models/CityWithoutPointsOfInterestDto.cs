﻿namespace CityInfo.API.Models
{
    /// <summary>
    /// A city without its points of interests
    /// </summary>
    public class CityWithoutPointsOfInterestDto
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
    }
}
