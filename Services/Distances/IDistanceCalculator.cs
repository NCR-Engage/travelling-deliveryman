namespace Ncr.TravellingDeliveryman.Services.Distances
{
    public interface IDistanceCalculator
    {
        double GetDistanceInKm(Coord a, Coord b);
    }
}
