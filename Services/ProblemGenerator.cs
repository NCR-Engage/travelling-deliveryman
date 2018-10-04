using System;
using System.Collections.Generic;

namespace Ncr.TravellingDeliveryman.Services
{
    public static class ProblemGenerator
    {
        public struct Step
        {
            public int orderId;
            public double latc;
            public double longc;
            public double latr;
            public double longr;

            public Step(int orderId, double latc, double longc, double latr, double longr)
            {
                this.orderId = orderId;
                this.latc = latc;
                this.longc = longc;
                this.latr = latr;
                this.longr = longr;
            }
        }

        public static IEnumerable<Step> GetProblem()
        {
            var r = new Random(5);

            var currentOrderId = 548884;
            for (var i = 0; i < 5000; i++)
            {
                currentOrderId += r.Next(10) + 1;
                var latc = 50.07 + (r.NextDouble() * 2 / 100);
                var longc = 14.43 + (r.NextDouble() * 2 / 100);
                var latr = 50.07 + (r.NextDouble() * 2 / 100);
                var longr = 14.43 + (r.NextDouble() * 2 / 100);

                yield return new Step(currentOrderId, latc, longc, latr, longr);
            }
        }
    }
}
