using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ncr.TravellingDeliveryman.Models
{
    public class IndexViewModel
    {
        public IFormFile Solution { get; set; }

        public decimal? Length { get; set; }

        public bool ProblemWithSolution { get; set; }
    }
}
