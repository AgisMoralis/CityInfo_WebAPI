﻿using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();

        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        /// <summary>
        /// NOTE: Explicitly defining here that the returned type is
        /// a nullable 'City?' serves two main purposes:
        /// 1) Explicitly defining that the returned result can be null,
        /// so that all developers are aware and handle it appropriately.
        /// 2) Enables compiler warnings if someone tries to use the returned
        /// type directly without checking it is null (safer code practice).
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);

        Task<bool> CityExistsAsync(int cityId);

        Task<bool> CityNameMatchesCityIdAsync(string? cityName, int cityId);

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestOfCityAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestOfCityAsync(int cityId, int pointOfInterestId);

        Task AddPointOfInterestOnCityAsync(int cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        Task<bool> SaveChangesAsync();
    }
}
