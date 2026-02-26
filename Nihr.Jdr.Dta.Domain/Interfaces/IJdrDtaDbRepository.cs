using Nihr.Jdr.Dta.Domain.Entities;

namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface IJdrDtaDbRepository
{
    Task<IDictionary<int, Person>> GetExistingVolunteersAsync(IList<int> volunteerIds,
        CancellationToken cancellationToken);

    Task AddNewVolunteerAsync(Person volunteer, CancellationToken cancellationToken);
    Task UpdateVolunteerAsync(Person existingVolunteer, Person incomingVolunteer, CancellationToken cancellationToken);

    Task PersistBatchAsync(CancellationToken cancellationToken);
    
    Task RemoveVolunteersNotInAsync(IReadOnlySet<int> incomingVolunteerIds, CancellationToken cancellationToken);
}