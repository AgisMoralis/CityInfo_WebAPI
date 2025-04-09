using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDatastore
    {
        public List<CityDto> Cities { get; set; }

        public static CitiesDatastore Current { get; set; } = new CitiesDatastore();

        public CitiesDatastore()
        {
            // Dummy data
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "NYC",
                    Description = "City with a lot of people",
                    PointsOfInterest = new List<PointOfInterestDto> ()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "Big park",
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Empire State",
                            Description = "Tall building",
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Athens",
                    Description = "Ancient City",
                    PointsOfInterest = new List<PointOfInterestDto> ()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Parthenon",
                            Description = "Ancient temple",
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Acropolis",
                            Description = "Center of the city",
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "City with Eiffel Tower",
                    PointsOfInterest = new List<PointOfInterestDto> ()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Eiffel Tower",
                            Description = "Tall tower",
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Louvre Museum",
                            Description = "World's biggest museum",
                        }
                    }
                }
            };
        }
    }
}
