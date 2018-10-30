using Ncr.TravellingDeliveryman.Services.Distances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncr.TravellingDeliveryman.Services
{
    public interface ISolutionEvaluator
    {
        double Evaluate(IEnumerable<int> solution);
    }

    public class SolutionEvaluator : ISolutionEvaluator
    {
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly IProblemGenerator _problemGenerator;

        public SolutionEvaluator(IDistanceCalculator distanceCalculator, IProblemGenerator problemGenerator)
        {
            _distanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
            _problemGenerator = problemGenerator ?? throw new ArgumentNullException(nameof(problemGenerator));
        }

        public double Evaluate(IEnumerable<int> solution)
        {
            var problem = _problemGenerator.GetProblem().ToDictionary(k => k.OrderId);
            var totalLength = 0.0;

            Coord? lastLocation = null;
            foreach (var stepOrderId in solution)
            {
                var step = problem[stepOrderId];

                if (lastLocation.HasValue)
                {
                    totalLength += _distanceCalculator.GetDistanceInKm(lastLocation.Value, step.RestaurantLocation);
                }

                totalLength += _distanceCalculator.GetDistanceInKm(step.CustomerLocation, step.RestaurantLocation);
                lastLocation = step.CustomerLocation;
            }

            return totalLength;
        }
    }
}
