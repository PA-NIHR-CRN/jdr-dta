using NIHR.Infrastructure.EntityFrameworkCore;

namespace Nihr.Jdr.Dta.Domain.Entities;

/// <summary>
/// This class represents a `volunteer` in the JDR database, however, for mapping to OMOP the table needs to be called `Person`.
/// </summary>
public class Person : ISoftDelete, ITimestamped
{
    public int Id { get; set; }
    public int JdrVolunteerId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Sex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public required Appointment Appointment { get; set; }
    public required Diagnosis Diagnosis { get; set; }
    public required Symptom Symptom { get; set; }
    public required Mmse Mmse { get; set; }
    public required Moca Moca { get; set; }
    public required Ace Ace { get; set; }
}