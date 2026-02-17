using NIHR.Infrastructure.EntityFrameworkCore;

namespace Nihr.Jdr.Dta.Domain.Entities;

public class Symptom : ISoftDelete, ITimestamped
{
    public int Id { get; set; }
    public Person Person { get; set; }
    public int PersonId { get; set; }

    public DateTime? StartDate { get; set; }
    public string? Level { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}