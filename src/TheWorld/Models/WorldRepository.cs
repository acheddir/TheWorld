﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using TheWorld.Caching;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private const string TRIPS_ALL_KEY = "___theWorld.trip.all___";
        private const string TRIPS_ALL_WITH_STOPS_KEY = "___theWorld.trip.all.with.stops___";
        private const string TRIPS_ALL_WITH_STOPS_FOR_USER_KEY = "___theWorld.trip.all.with.stops.{0}___";

        public WorldRepository(WorldContext context, ICacheManager cacheManager)
        {
            _context = context;
            _cacheManager = cacheManager;
        }

        public void ClearCache(string username)
        {
            var keys = new List<string>
            {
                TRIPS_ALL_KEY,
                TRIPS_ALL_WITH_STOPS_KEY,
                string.Format(TRIPS_ALL_WITH_STOPS_FOR_USER_KEY, username),
            };

            foreach (var key in keys)
            {
                _cacheManager.Remove(key);
            }
        }

        #region Implementation of IWorldRepository

        public IEnumerable<Trip> GetAllTrips()
        {
            string key = TRIPS_ALL_KEY;

            return _cacheManager.Get(key, () =>
            {
                return _context.Trips
                    .OrderBy(t => t.Name)
                    .ToList();
            });
        }

        public IEnumerable<Trip> GetAllTripsWithStops()
        {
            string key = TRIPS_ALL_WITH_STOPS_KEY;

            return _cacheManager.Get(key, () =>
            {
                return _context.Trips
                .Include(t => t.Stops)
                .OrderBy(t => t.Name)
                .ToList();
            });
        }

        public IEnumerable<Trip> GetUserTripsWithStops(string username)
        {
            string key = string.Format(TRIPS_ALL_WITH_STOPS_FOR_USER_KEY, username);

            return _cacheManager.Get(key, () =>
            {
                return _context.Trips
                    .Where(t => t.UserName == username)
                    .Include(t => t.Stops)
                    .OrderBy(t => t.Name)
                    .ToList();
            });
        }

        public Trip GetTripByName(string tripName, string username)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .FirstOrDefault(t => t.Name == tripName && t.UserName == username);
        }

        public Stop GetStopByName(string tripName, string stopName, string username)
        {
            var trip = this.GetTripByName(tripName, username);
            var stop = trip.Stops.FirstOrDefault(s => s.Name == stopName);
            return stop;
        }

        public void AddTrip(Trip newTrip)
        {
            _context.Add(newTrip);
        }

        public void AddStop(string tripName, string username, Stop newStop)
        {
            var theTrip = this.GetTripByName(tripName, username);
            newStop.Order = theTrip.Stops.Max(s => (int?)s.Order) ?? 0 + 1;
            theTrip.Stops.Add(newStop);
            _context.Add(newStop);
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }

        #endregion

        #region Fields

        private WorldContext _context;
        private ICacheManager _cacheManager;

        #endregion
    }
}