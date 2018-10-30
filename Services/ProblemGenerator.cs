using Ncr.TravellingDeliveryman.Services.Distances;
using System;
using System.Collections.Generic;

namespace Ncr.TravellingDeliveryman.Services
{
    public interface IProblemGenerator
    {
        IEnumerable<ProblemStep> GetProblem();
    }

    public class ProblemGenerator : IProblemGenerator
    {
        public static readonly int StepCount = 15000;

        public IEnumerable<ProblemStep> GetProblem()
        {
            var r = new Random(5);

            var currentOrderId = 548884;
            for (var i = 0; i < StepCount; i++)
            {
                currentOrderId += r.Next(10) + 1;
                var latc = 50.07 + (r.NextDouble() * 2 / 20);
                var longc = 14.43 + (r.NextDouble() * 2 / 20);
                var latr = 50.07 + (r.NextDouble() * 2 / 20);
                var longr = 14.43 + (r.NextDouble() * 2 / 20);

                yield return new ProblemStep {
                    OrderId = currentOrderId,
                    CustomerLocation = new Coord { Lat = latc, Long = longc },
                    RestaurantLocation = new Coord { Lat = latr, Long = longr } };
            }
        }
    }
}
