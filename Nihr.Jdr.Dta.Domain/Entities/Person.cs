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

    public Appointment Appointment { get; set; }
    public Diagnosis Diagnosis { get; set; }
    public Symptom Symptom { get; set; }
    public Mmse Mmse { get; set; }
    public Moca Moca { get; set; }
    public Ace Ace { get; set; }
}