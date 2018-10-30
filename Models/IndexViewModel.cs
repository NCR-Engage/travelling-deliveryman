using Microsoft.AspNetCore.Http;
using Ncr.TravellingDeliveryman.Repositories;
using System.Collections.Generic;

namespace Ncr.TravellingDeliveryman.Models
{
    public class IndexViewModel
    {
        public bool Kiosk { get; set; }

        public string Name { get; set; }

        public string EMail { get; set; }

        public string OpenDoor { get; set; }

        public IFormFile Solution { get; set; }

        public double? Length { get; set; }
        
        public bool ProblemWithSolution { get; set; }

        public bool ProblemWithRegistration { get; set; }

        public IList<SolutionlessSolution> BestSolutions { get; set; }
    }
}
