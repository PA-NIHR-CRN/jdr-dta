using Nihr.Jdr.Dta.Domain.Entities;

namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface IExternalJdrDbRepository
{
    IAsyncEnumerable<Person> GetExternalVolunteerAsync(CancellationToken cancellationToken);
}