using NIHR.Infrastructure.EntityFrameworkCore;

namespace Nihr.Jdr.Dta.Domain.Entities;

public class Ace : ISoftDelete, ITimestamped
{
    public int Id { get; set; }
    public Person Person { get; set; } = null!;
    public int PersonId { get; set; }


    public DateTime? Date { get; set; }
    public int? Score { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}