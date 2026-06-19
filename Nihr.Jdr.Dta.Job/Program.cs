using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nihr.Jdr.Dta.Job.CarrotCdm;
using Nihr.Jdr.Dta.Job.Startup;
using Nihr.Jdr.Dta.Job.Utility;

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

            // Argument Parsing
            if (args.Length > 0)
            {
                using var scope = host.Services.CreateScope();
                var utilityRunner = scope.ServiceProvider.GetRequiredService<UtilityJobRunner>();

                var command = args[0].ToLowerInvariant();
                return command switch
                {
                    "--deploy-ddl" => await utilityRunner.DeployDdlAsync(),
                    "--load-ref-data" => await utilityRunner.LoadReferenceDataAsync(args.Skip(1).ToArray()),
                    _ => RunInvalidCommand(logger, command)
                };
            }

            // Default behavior (Standard ECS Job)
            logger.LogInformation("No arguments provided. Starting standard job execution.");
            return await RunStandardJobAsync(host, logger);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync("Critical error during application startup:");
            Console.Error.WriteLine(e);
            return 1;
        }
    }

    private static async Task<int> RunStandardJobAsync(IHost host, ILogger logger)
    {
        using var scope = host.Services.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService<JobRunner>();
        var carrotRunner = scope.ServiceProvider.GetRequiredService<CarrotCdmTransformJob>();

        var loadResult = await job.RunAsync();
        if (loadResult != 0)
        {
            logger.LogWarning("Skipping CarrotCDM run because job failed");
            return loadResult;
        }

        return await carrotRunner.RunAsync();
    }

    private static int RunInvalidCommand(ILogger logger, string command)
    {
        logger.LogError("Unknown command: {Command}. Available: --deploy-ddl, --load-ref-data", command);
        return 1;
    }
}