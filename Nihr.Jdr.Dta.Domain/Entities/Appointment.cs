using NIHR.Infrastructure.EntityFrameworkCore;

namespace Nihr.Jdr.Dta.Domain.Entities;

public class Appointment : ISoftDelete, ITimestamped
{
    public int Id { get; set; }

    public Person Person { get; set; } = null!;
    public int PersonId { get; set; }


    public DateTime? DeclarationAcceptedDate { get; set; }
    public string? Disabilities { get; set; }
    public string? OtherDisabilities { get; set; }
    public string? MedicalConditions { get; set; }
    public string? OtherMedicalConditions { get; set; }
    public string? DementiaHistoryFamily { get; set; }
    public bool SuffersMemoryProblems { get; set; }
    public string? EthnicGroup { get; set; }
    public string? Region { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}