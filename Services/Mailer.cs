using Microsoft.Extensions.Options;
using Ncr.TravellingDeliveryman.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ncr.TravellingDeliveryman.Services
{
    public class Mailer
    {
        private readonly MailgunConfiguration _mailgunConfiguration;

        public Mailer(IOptions<MailgunConfiguration> mailgunConfiguration)
        {
            _mailgunConfiguration = mailgunConfiguration?.Value ?? throw new ArgumentNullException(nameof(mailgunConfiguration));
        }

        public async Task<bool> SendRegistrationReceiptToUs(string name, string mail, string openDoor)
        {
            var messageForUs = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{_mailgunConfiguration.Server}/messages");
            messageForUs.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_mailgunConfiguration.Key}")));

            messageForUs.Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("from", _mailgunConfiguration.Sender),
                    new KeyValuePair<string, string>("to", _mailgunConfiguration.ReceiptTo),
                    new KeyValuePair<string, string>("bcc", _mailgunConfiguration.BccLogger),
                    new KeyValuePair<string, string>("subject", "Registrace z job fairu"),
                    new KeyValuePair<string, string>("html", $"Jméno: {WebUtility.HtmlEncode(name)}<br>E-mail: {WebUtility.HtmlEncode(mail)}<br>Open door: {openDoor}")
                });

            var response = await new HttpClient().SendAsync(messageForUs);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> SendWelcomeMailToThem(string mail)
        {
            var messageForUs = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{_mailgunConfiguration.Server}/messages");
            messageForUs.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_mailgunConfiguration.Key}")));

            messageForUs.Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("from", _mailgunConfiguration.Sender),
                    new KeyValuePair<string, string>("to", mail),
                    new KeyValuePair<string, string>("bcc", _mailgunConfiguration.BccLogger),
                    new KeyValuePair<string, string>("h:Reply-To", _mailgunConfiguration.WelcomeReplyTo),
                    new KeyValuePair<string, string>("subject", "With regards to the job fair"),
                    new KeyValuePair<string, string>("html", @"<p>Hello!

<p>Thank you for being interested in NCR engineering opportunities! We are sending you this mail as a confirmation of your registration today at our job fair stand.

<p>If you want to take a place in the programming contest for Sport Tester Watch, proceed to https://contest.ncrlab.cz . You have less than week to submit your solution!

<p>Feel free to reply to this mail and ask any questions you have about the contest or about NCR’s Prague office.

<p>Best regards,
<p>NCR Prague office")
                });

            var response = await new HttpClient().SendAsync(messageForUs);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public void SolutionAccepted(string mail, string solution)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", _mailgunConfiguration.Key);

            var request = new RestRequest();
            request.Resource = $"{_mailgunConfiguration.Server}/messages";
            request.AddParameter("domain", "mg.ncrlab.cz", ParameterType.UrlSegment);
            request.AddParameter("from", _mailgunConfiguration.Sender);
            request.AddParameter("to", mail);
            request.AddParameter("bcc", $"{_mailgunConfiguration.SolutionAcceptedToBcc};{_mailgunConfiguration.BccLogger}");
            request.AddParameter("subject", "Solution accepted");
            request.AddParameter("html", "<p>Hello!<p>We accepted your solution for Travelling Deliveryman contest. As soon as the contest is over, we will update you with the final results.");
            request.AddFile("yoursolution.txt", Encoding.UTF8.GetBytes(solution), "yoursolution.txt");
            request.Method = Method.POST;
            client.Execute(request);
        }
    }
}
