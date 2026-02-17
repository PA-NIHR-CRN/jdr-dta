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

    public async Task<int> RunAsync(CancellationToken ct = default)
    {
        int totalProcessed = 0;
        var batch = new List<Person>();

        // Consume the transformed stream
        await foreach (var person in externalJdrDbRepository.GetExternalVolunteerAsync(ct))
        {
            batch.Add(person);

            if (batch.Count >= BatchSize)
            {
                await ProcessBatch(batch, ct);
                totalProcessed += batch.Count;
                logger.LogInformation("Synchronized {Count} records...", totalProcessed);
                batch.Clear();
            }
        }

        if (batch.Any())
        {
            await ProcessBatch(batch, ct);
            totalProcessed += batch.Count;
        }

        return 0;
    }

    private async Task ProcessBatch(List<Person> batch, CancellationToken ct)
    {
        var volunteerIds = batch.Select(b => b.JdrVolunteerId).ToList();

        // Load existing records to determine if we Update or Insert
        var existingPersons = await jdrDtaDbRepository.GetExistingVolunteersAsync(volunteerIds, ct);

        foreach (var incoming in batch)
        {
            if (existingPersons.TryGetValue(incoming.JdrVolunteerId, out var existing))
            {
                await jdrDtaDbRepository.UpdateVolunteerAsync(existing, incoming, ct);
            }
            else
            {
                await jdrDtaDbRepository.AddNewVolunteerAsync(incoming, ct);
            }
        }

        await jdrDtaDbRepository.PersistBatchAsync(ct);
    }
}