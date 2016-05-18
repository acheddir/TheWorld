using System.Collections.Generic;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        void ClearCache(string username);
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetAllTripsWithStops();
        IEnumerable<Trip> GetUserTripsWithStops(string username);
        Trip GetTripByName(string tripName, string username);
        void AddTrip(Trip newTrip);
        void AddStop(string tripName, string username, Stop newStop);
        bool SaveAll();
    }
}