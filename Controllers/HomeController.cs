using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ncr.TravellingDeliveryman.Models;
using Ncr.TravellingDeliveryman.Repositories;
using Ncr.TravellingDeliveryman.Services;

namespace Ncr.TravellingDeliveryman.Controllers
{
    public class HomeController : Controller
    {
        private readonly Mailer _mailer;
        private readonly ISolutionEvaluator _solutionEvaluator;
        private readonly IProblemGenerator _problemGenerator;
        private readonly SolutionRepository _solutionRepository;
        private readonly RegistrationRepository _registrationRepository;

        public HomeController(Mailer mailer, ISolutionEvaluator solutionEvaluator,
            IProblemGenerator problemGenerator, SolutionRepository solutionRepository,
            RegistrationRepository registrationRepository)
        {
            _mailer = mailer ?? throw new ArgumentNullException(nameof(mailer));
            _solutionEvaluator = solutionEvaluator ?? throw new ArgumentNullException(nameof(solutionEvaluator));
            _problemGenerator = problemGenerator ?? throw new ArgumentNullException(nameof(problemGenerator));
            _solutionRepository = solutionRepository ?? throw new ArgumentNullException(nameof(solutionRepository));
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
        }

        public async Task<IActionResult> Index(IndexViewModel submittedSolution, [FromQuery]bool kiosk)
        {
            string submittedText = null;
            if (submittedSolution.Solution != null)
            {
                try
                {
                    using (var sr = new StreamReader(submittedSolution.Solution.OpenReadStream()))
                    {
                        submittedText = await sr.ReadToEndAsync();
                        var solution = submittedText.Split('\n').Select(l => l.Trim()).Where(l => l != "").Select(l => int.Parse(l)).Distinct().ToList();

                        if (solution.Count != ProblemGenerator.StepCount)
                        {
                            throw new Exception();
                        }

                        submittedSolution.Length = _solutionEvaluator.Evaluate(solution);
                        
                        _mailer.SolutionAccepted(submittedSolution.EMail, submittedText);
                        
                        await _solutionRepository.InsertSolution(new Repositories.Solution
                        {
                            Created = DateTime.Now,
                            EMail = submittedSolution.EMail,
                            Name = submittedSolution.Name,
                            Length = Convert.ToDecimal(submittedSolution.Length.Value),
                            Submission = submittedText
                        });
                    }
                }
                catch (Exception ex)
                {
                    submittedSolution.ProblemWithSolution = true;
                    throw;
                }
            }

            if (kiosk && !string.IsNullOrEmpty(submittedSolution.Name) && !string.IsNullOrEmpty(submittedSolution.EMail))
            {
                bool atLeastOneSucces = false;

                try
                {
                    await _registrationRepository.InsertRegistration(new Registration
                    {
                        EMail = submittedSolution.EMail,
                        Name = submittedSolution.Name,
                        OpenDoors = submittedSolution.OpenDoor
                    });
                    atLeastOneSucces = true;
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var receiptResult = await _mailer.SendRegistrationReceiptToUs(submittedSolution.Name, submittedSolution.EMail, submittedSolution.OpenDoor);
                    var welcomeResult = await _mailer.SendWelcomeMailToThem(submittedSolution.EMail);
                    atLeastOneSucces = atLeastOneSucces || receiptResult || welcomeResult;
                }
                catch (Exception ex)
                {

                }


                if (atLeastOneSucces && kiosk)
                {
                    return Redirect("/Home/RegistrationSuccess");
                }
                else if (kiosk)
                {
                    submittedSolution.ProblemWithRegistration = true;
                }
            }
            
            if (!kiosk)
            {
                try
                {
                    submittedSolution.BestSolutions = await _solutionRepository.GetBestSolutions();
                }
                catch (Exception ex)
                {

                }
            }

            submittedSolution.Kiosk = kiosk;

            return View(submittedSolution);
        }

        public string List()
        {
            var list = new StringBuilder();
            
            foreach (var order in _problemGenerator.GetProblem())
            {
                list.Append($"{order.OrderId};{order.CustomerLocation.Lat:0.#######}N,{order.CustomerLocation.Long:0.#######}E;{order.RestaurantLocation.Lat:0.#######}N,{order.RestaurantLocation.Long:0.#######}E\r\n");
            }

            return list.ToString();
        }

        public string Solution()
        {
            var list = new StringBuilder();

            foreach (var order in _problemGenerator.GetProblem())
            {
                list.Append($"{order.OrderId}\r\n");
            }

            return list.ToString();
        }

        public IActionResult RegistrationSuccess()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
