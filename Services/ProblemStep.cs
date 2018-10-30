using Ncr.TravellingDeliveryman.Services.Distances;

namespace Ncr.TravellingDeliveryman.Services
{
    public struct ProblemStep
    {
        public int OrderId { get; set; }

        public Coord CustomerLocation { get; set; }

        public Coord RestaurantLocation { get; set; }
    }

}
