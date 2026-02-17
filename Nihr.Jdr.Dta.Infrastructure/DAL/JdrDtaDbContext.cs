using Microsoft.EntityFrameworkCore;
using NIHR.Infrastructure.EntityFrameworkCore;
using Nihr.Jdr.Dta.Domain.Entities;

namespace Nihr.Jdr.Dta.Infrastructure.DAL;

public class JdrDtaDbContext(DbContextOptions<JdrDtaDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNihrConventions();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(builder =>
        {
            builder.HasOne(p => p.Appointment)
                .WithOne(a => a.Person)
                .HasForeignKey<Appointment>(a => a.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Diagnosis)
                .WithOne(d => d.Person)
                .HasForeignKey<Diagnosis>(d => d.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Symptom)
                .WithOne(s => s.Person)
                .HasForeignKey<Symptom>(s => s.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Mmse)
                .WithOne(m => m.Person)
                .HasForeignKey<Mmse>(m => m.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Moca)
                .WithOne(m => m.Person)
                .HasForeignKey<Moca>(m => m.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Ace)
                .WithOne(a => a.Person)
                .HasForeignKey<Ace>(a => a.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Diagnosis> Diagnosis { get; set; }
    public DbSet<Mmse> Mmse { get; set; }
    public DbSet<Ace> Ace { get; set; }
    public DbSet<Moca> Moca { get; set; }
    public DbSet<Symptom> Symptoms { get; set; }
}