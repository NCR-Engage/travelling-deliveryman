using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ncr.TravellingDeliveryman.Models;
using Ncr.TravellingDeliveryman.Services;
using travelling_deliveryman.Models;

namespace travelling_deliveryman.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(IndexViewModel submittedSolution, [FromQuery]bool kiosk)
        {
            if (submittedSolution?.Solution != null)
            {
                try
                {
                    using (var sr = new StreamReader(submittedSolution.Solution.OpenReadStream()))
                    {
                        var text = await sr.ReadToEndAsync();
                        var solution = text.Split('\n').Select(l => l.Trim()).Where(l => l != "").Select(l => int.Parse(l)).ToList();

                        if (solution.Count != 5000)
                        {
                            throw new Exception();
                        }

                        submittedSolution.Length = SolutionEvaluator.Evaluate(solution);
                    }
                }
                catch
                {
                    submittedSolution.ProblemWithSolution = true;
                }
            }

            submittedSolution.Kiosk = kiosk;

            return View(submittedSolution);
        }

        public string List()
        {
            var list = new StringBuilder();
            
            foreach (var order in ProblemGenerator.GetProblem())
            {
                list.Append($"{order.orderId};{order.latc:0.#######}N,{order.longc:0.#######}E;{order.latr:0.#######}N,{order.longr:0.#######}E\r\n");
            }

            return list.ToString();
        }

        public string Solution()
        {
            var list = new StringBuilder();

            foreach (var order in ProblemGenerator.GetProblem())
            {
                list.Append($"{order.orderId}\r\n");
            }

            return list.ToString();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
