using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Infrastructure.Settings;

namespace Nihr.Jdr.Dta.Job.CarrotCdm;

public sealed class CarrotTransformRunner(
    ILogger<CarrotTransformRunner> logger,
    IOptions<CarrotCdmSettings> carrotCdmSettings)
{
    private readonly CarrotCdmSettings _settings = carrotCdmSettings.Value;
    private const string PythonPath = "/opt/carrot-venv/bin/python";

    public async Task<int> RunAsync()
    {
        var psi = CreateProcessStartInfo();

        using var process = new Process();
        process.StartInfo = psi;

        logger.LogInformation("Starting CarrotCDM transform");

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                logger.LogInformation("[CarrotCDM] {Line}", e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                logger.LogError("[CarrotCDM] {Line}", e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            logger.LogError(
                "CarrotCDM failed with exit code {ExitCode}",
                process.ExitCode);

            return process.ExitCode;
        }

        logger.LogInformation("CarrotCDM completed successfully");
        return 0;
    }

    private ProcessStartInfo CreateProcessStartInfo()
    {
        return new ProcessStartInfo
        {
            FileName = PythonPath,
            Arguments = BuildArguments(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    }

    private string BuildArguments()
    {
        return string.Join(" ",
            "-m", "carrottransform.cli.command",
            "run", "mapstream",
            "--inputs", Quote(_settings.InputDirectory),
            "--rules-file", Quote(_settings.RulesFile),
            "--person", Quote(_settings.PersonTable),
            "--output", Quote(_settings.OutputDirectory),
            "--omop-ddl-file", Quote(_settings.DdlFile),
            "--omop-config-file", Quote(_settings.ConfigFile));
    }

    private static string Quote(string value) => $"\"{value}\"";
}