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
                    Description = "City with a lot of people"
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "City with a big port"
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Antwerp",
                    Description = "City with Eiffel Tower"
                }
            };
        }
    }
}
