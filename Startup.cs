using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ncr.TravellingDeliveryman.Configuration;
using Ncr.TravellingDeliveryman.Repositories;
using Ncr.TravellingDeliveryman.Services;
using Ncr.TravellingDeliveryman.Services.Distances;

namespace travelling_deliveryman
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<Mailer>();
            services.AddTransient<ISolutionEvaluator, SolutionEvaluator>();
            services.AddTransient<IProblemGenerator, ProblemGenerator>();
            services.AddTransient<SolutionRepository>();
            services.AddTransient<RegistrationRepository>();
            services.AddSingleton<IDistanceCalculator, NaiveDistanceCalculator>();
            services.Configure<MailgunConfiguration>(Configuration.GetSection("Mailgun"));
            services.Configure<DbConfiguration>(Configuration.GetSection("Db"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
