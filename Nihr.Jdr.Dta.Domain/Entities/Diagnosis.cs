using NIHR.Infrastructure.EntityFrameworkCore;

namespace Nihr.Jdr.Dta.Domain.Entities;

public class Diagnosis : ISoftDelete, ITimestamped
{
    public int Id { get; set; }

    public Person Person { get; set; } = null!;
    public int PersonId { get; set; }


    public DateTime? DiagnosisDate { get; set; }
    public string? DiagnosisName { get; set; }
    public string? SubtypeDiagnosisName { get; set; }
    public string? PositiveAmyloidPlaque { get; set; }
    public string? PositiveApoe4 { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}