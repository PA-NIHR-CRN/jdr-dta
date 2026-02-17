using Microsoft.EntityFrameworkCore;
using Nihr.Jdr.Dta.Domain.Entities;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.DAL;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public class JdrDtaDbRepository(JdrDtaDbContext dbContext) : IJdrDtaDbRepository
{
    public async Task<IDictionary<int, Person>> GetExistingVolunteersAsync(IList<int> volunteerIds,
        CancellationToken cancellationToken)
    {
        return await dbContext.Persons
            .Where(p => volunteerIds.Contains(p.JdrVolunteerId))
            .ToDictionaryAsync(p => p.JdrVolunteerId, cancellationToken);
    }

    public Task AddNewVolunteerAsync(Person volunteer, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateVolunteerAsync(Person existingVolunteer, Person incomingVolunteer, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task PersistBatchAsync(CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);

        // Critical: Clear tracker to maintain performance during large jobs
        dbContext.ChangeTracker.Clear();
    }
}