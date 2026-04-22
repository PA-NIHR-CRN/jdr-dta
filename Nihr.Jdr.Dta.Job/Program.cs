using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nihr.Jdr.Dta.Job.CarrotCdm;
using Nihr.Jdr.Dta.Job.Startup;

namespace Nihr.Jdr.Dta.Job;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.AddNihrConfiguration();
            builder.ConfigureNihrLogging();
            builder.ConfigureDependencyInjection();

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Application host built. Starting job execution");

            using var scope = host.Services.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<JobRunner>();
            var carrotRunner = scope.ServiceProvider.GetRequiredService<CarrotCdmOrchestrator>();

            var loadResult = await job.RunAsync();

            logger.LogInformation("Job execution finished with result: {Result}", loadResult);

            if (loadResult != 0)
            {
                logger.LogWarning("Skipping CarrotCDM run because job failed");
                return loadResult;
            }
            
            var carrotResult = await carrotRunner.RunAsync();

            logger.LogInformation("CarrotCDM finished with result: {Result}", carrotResult);

            return carrotResult;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Critical error during application startup:");
            Console.Error.WriteLine(e);
            return 1;
        }
    }
}