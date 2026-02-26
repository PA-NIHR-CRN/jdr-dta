using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nihr.Jdr.Dta.Domain.Entities;
using Nihr.Jdr.Dta.Domain.Interfaces;

namespace Nihr.Jdr.Dta.Job;

public class JobRunner(
    IExternalJdrDbRepository externalJdrDbRepository,
    IJdrDtaDbRepository jdrDtaDbRepository,
    ILogger<JobRunner> logger)
{
    private const int BatchSize = 500;
    private readonly HashSet<int> _incomingVolunteerIds = [];
    
    public async Task<int> RunAsync(CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Job started at {StartTime}", DateTime.UtcNow);
        int totalProcessed = 0;
        var batch = new List<Person>();

        try
        {
            // Consume the transformed stream
            await foreach (var person in externalJdrDbRepository.GetExternalVolunteerAsync(ct))
            {
                batch.Add(person);

                if (batch.Count >= BatchSize)
                {
                    await ProcessBatch(batch, ct);
                    totalProcessed += batch.Count;
                    logger.LogInformation("Processed {Count} records so far...", totalProcessed);
                    batch.Clear();
                }
            }

            if (batch.Any())
            {
                await ProcessBatch(batch, ct);
                totalProcessed += batch.Count;
            }

            await jdrDtaDbRepository.RemoveVolunteersNotInAsync(_incomingVolunteerIds, ct);

            stopwatch.Stop();

            logger.LogInformation(
                "Job completed successfully at {EndTime}. Total records processed: {Total}. Total time taken: {ElapsedMs}ms",
                DateTime.UtcNow,
                totalProcessed,
                stopwatch.ElapsedMilliseconds);

            return 0;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Job was cancelled after processing {Total} records", totalProcessed);
            return 1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Job failed after processing {Total} records", totalProcessed);
            return 1;
        }
    }

    private async Task ProcessBatch(List<Person> batch, CancellationToken ct)
    {
        var volunteerIds = batch.Select(b => b.JdrVolunteerId).ToList();

        logger.LogDebug("Processing batch of {Count} volunteers", batch.Count);

        // Load existing records to determine if we Update or Insert
        var existingPersons = await jdrDtaDbRepository.GetExistingVolunteersAsync(volunteerIds, ct);

        int added = 0;
        int updated = 0;

        foreach (var incoming in batch)
        {
            _incomingVolunteerIds.Add(incoming.JdrVolunteerId);
            
            if (existingPersons.TryGetValue(incoming.JdrVolunteerId, out var existing))
            {
                await jdrDtaDbRepository.UpdateVolunteerAsync(existing, incoming, ct);
                updated++;
            }
            else
            {
                await jdrDtaDbRepository.AddNewVolunteerAsync(incoming, ct);
                added++;
            }
        }

        logger.LogDebug("Batch summary: {Added} added, {Updated} updated", added, updated);

        await jdrDtaDbRepository.PersistBatchAsync(ct);
    }
}