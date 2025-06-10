using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            // We should get rid of that return, because we always want to apply paging in the returned data
            // if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(searchQuery))
            // {
            //     return await GetCitiesAsync();
            // }

            var collectionOfCities = _context.Cities as IQueryable<City>;

            // Filtering and Searching applied here
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collectionOfCities = collectionOfCities.Where(c => c.Name == name);
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collectionOfCities = collectionOfCities
                    .Where(c => c.Name.Contains(searchQuery) || (c.Description != null && c.Description.Contains(searchQuery)));
            }

            // Paging applied here
            var totalAmountOfItems = await collectionOfCities.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalAmountOfItems, pageSize, pageNumber);
            var collectionToReturn = await collectionOfCities
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _context.Cities
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<bool> CityNameMatchesCityIdAsync(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Name == cityName && c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestOfCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest
                    .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                    .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestOfCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                   .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestOnCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _ = _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
