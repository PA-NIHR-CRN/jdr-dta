using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nihr.Jdr.Dta.Domain.Entities;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.DAL;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public class JdrDtaDbRepository(
    JdrDtaDbContext dbContext,
    ILogger<JdrDtaDbRepository> logger)
    : IJdrDtaDbRepository
{
    public async Task<IDictionary<int, Person>> GetExistingVolunteersAsync(IList<int> volunteerIds,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Fetching {Count} existing volunteers from staging database", volunteerIds.Count);

        // Make sure to include the related entities, otherwise any update operation later will fail.
        return await dbContext.Persons
            .Include(p => p.Appointment)
            .Include(p => p.Diagnosis)
            .Include(p => p.Symptom)
            .Include(p => p.Mmse)
            .Include(p => p.Moca)
            .Include(p => p.Ace)
            .Where(p => volunteerIds.Contains(p.JdrVolunteerId))
            .ToDictionaryAsync(p => p.JdrVolunteerId, cancellationToken);
    }

    public async Task AddNewVolunteerAsync(Person volunteer, CancellationToken ct)
    {
        await dbContext.Persons.AddAsync(volunteer, ct);
    }

    public Task UpdateVolunteerAsync(Person existingVolunteer, Person incomingVolunteer, CancellationToken ct)
    {
        // These updates will get picked up by the EF change tracker.
        existingVolunteer.DateOfBirth = incomingVolunteer.DateOfBirth;
        existingVolunteer.Sex = incomingVolunteer.Sex;

        existingVolunteer.Appointment.DeclarationAcceptedDate = incomingVolunteer.Appointment.DeclarationAcceptedDate;
        existingVolunteer.Appointment.Disabilities = incomingVolunteer.Appointment.Disabilities;
        existingVolunteer.Appointment.OtherDisabilities = incomingVolunteer.Appointment.OtherDisabilities;
        existingVolunteer.Appointment.MedicalConditions = incomingVolunteer.Appointment.MedicalConditions;
        existingVolunteer.Appointment.OtherMedicalConditions = incomingVolunteer.Appointment.OtherMedicalConditions;
        existingVolunteer.Appointment.DementiaHistoryFamily = incomingVolunteer.Appointment.DementiaHistoryFamily;
        existingVolunteer.Appointment.SuffersMemoryProblems = incomingVolunteer.Appointment.SuffersMemoryProblems;
        existingVolunteer.Appointment.EthnicGroup = incomingVolunteer.Appointment.EthnicGroup;
        existingVolunteer.Appointment.Region = incomingVolunteer.Appointment.Region;

        existingVolunteer.Diagnosis.DiagnosisDate = incomingVolunteer.Diagnosis.DiagnosisDate;
        existingVolunteer.Diagnosis.DiagnosisName = incomingVolunteer.Diagnosis.DiagnosisName;
        existingVolunteer.Diagnosis.SubtypeDiagnosisName = incomingVolunteer.Diagnosis.SubtypeDiagnosisName;
        existingVolunteer.Diagnosis.PositiveAmyloidPlaque = incomingVolunteer.Diagnosis.PositiveAmyloidPlaque;
        existingVolunteer.Diagnosis.PositiveApoe4 = incomingVolunteer.Diagnosis.PositiveApoe4;

        existingVolunteer.Symptom.StartDate = incomingVolunteer.Symptom.StartDate;
        existingVolunteer.Symptom.Level = incomingVolunteer.Symptom.Level;

        existingVolunteer.Mmse.Date = incomingVolunteer.Mmse.Date;
        existingVolunteer.Mmse.Score = incomingVolunteer.Mmse.Score;

        existingVolunteer.Moca.Date = incomingVolunteer.Moca.Date;
        existingVolunteer.Moca.Score = incomingVolunteer.Moca.Score;

        existingVolunteer.Ace.Date = incomingVolunteer.Ace.Date;
        existingVolunteer.Ace.Score = incomingVolunteer.Ace.Score;

        return Task.CompletedTask;
    }

    public async Task PersistBatchAsync(CancellationToken ct)
    {
        try
        {
            logger.LogDebug("Persisting batch to staging database");
            var entriesAffected = await dbContext.SaveChangesAsync(ct);
            logger.LogDebug("Successfully persisted batch. {EntriesAffected} entries affected", entriesAffected);
            dbContext.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist batch to staging database");
            throw;
        }
    }

    public async Task RemoveVolunteersNotInAsync(IReadOnlySet<int> incomingVolunteerIds, CancellationToken cancellationToken)
    {
        var activeVolunteerIds = await dbContext.Persons
            .Where(p => !p.IsDeleted)
            .Select(p => p.JdrVolunteerId)
            .ToListAsync(cancellationToken);

        var volunteerIdsToRemove = activeVolunteerIds
            .Where(id => !incomingVolunteerIds.Contains(id))
            .ToList();

        if (volunteerIdsToRemove.Count == 0)
        {
            logger.LogDebug("No volunteers to remove");
            return;
        }
        
        logger.LogDebug("Removing {Count} volunteers", volunteerIdsToRemove.Count);

        var volunteersToRemove = await dbContext.Persons
            .Where(p => !p.IsDeleted)
            .Where(p => volunteerIdsToRemove.Contains(p.JdrVolunteerId))
            .ToListAsync(cancellationToken);
        
        dbContext.Persons.RemoveRange(volunteersToRemove);

        await PersistBatchAsync(cancellationToken);
    }
}