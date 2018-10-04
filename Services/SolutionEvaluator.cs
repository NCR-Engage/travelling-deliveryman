using System;
using System.Collections.Generic;
using System.Linq;
using static Ncr.TravellingDeliveryman.Services.ProblemGenerator;

namespace Ncr.TravellingDeliveryman.Services
{
    public static class SolutionEvaluator
    {
        public static decimal Evaluate(IEnumerable<int> solution)
        {
            var problem = GetProblem().ToDictionary(k => k.orderId);
            var totalLength = 0M;

            Step? lastStep = null;
            foreach (var stepOrderId in solution)
            {
                var step = problem[stepOrderId];

                if (lastStep != null)
                {
                    totalLength += GetDistance(new Step(0, lastStep.Value.latr, lastStep.Value.longr, step.latc, step.longc));
                }

                totalLength += GetDistance(step);
                lastStep = step;
            }

            return totalLength;
        }

        private static decimal GetDistance(Step s)
        {
            var latDistPer = 111.0;
            var longDistPer = 71.0;

            var latDist = (s.latc - s.latr) * latDistPer;
            var longDist = (s.longc - s.longr) * longDistPer;

            return Convert.ToDecimal(Math.Sqrt(latDist * latDist + longDist * longDist));
        }
    }
}
