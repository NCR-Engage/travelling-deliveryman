using System;

namespace Ncr.TravellingDeliveryman.Services.Distances
{
    public class NaiveDistanceCalculator : IDistanceCalculator
    {
        public double GetDistanceInKm(Coord a, Coord b)
        {
            var latDistPer = 111.0;
            var longDistPer = 71.0;

            var latDist = (a.Lat - b.Lat) * latDistPer;
            var longDist = (a.Long - b.Long) * longDistPer;

            return Math.Sqrt(latDist * latDist + longDist * longDist);
        }
    }
}
