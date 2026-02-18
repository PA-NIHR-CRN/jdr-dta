using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Nihr.Jdr.Dta.Domain.Entities;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Constants;
using Nihr.Jdr.Dta.Infrastructure.Helpers;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public class ExternalJdrDbRepository(
    IExternalJdrDbCredentialProvider externalJdrDbCredentialProvider,
    ILogger<ExternalJdrDbRepository> logger)
    : IExternalJdrDbRepository
{
    public async IAsyncEnumerable<Person> GetExternalVolunteerAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting retrieval of external volunteers from JDR database");

        string connectionString;
        try
        {
            connectionString = await externalJdrDbCredentialProvider.GetConnectionString(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve connection string for external JDR database");
            throw;
        }

        await using var connection = new MySqlConnection(connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open connection to external JDR database");
            throw;
        }

        logger.LogDebug("Executing query to fetch volunteers: {Query}", ExternalJdrDbQueries.GetVolunteers);
        await using var command = new MySqlCommand(ExternalJdrDbQueries.GetVolunteers, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var ordVolunteerId = reader.GetOrdinal(ExternalJdrDbColumns.VolunteerId);
        var ordDateOfBirth = reader.GetOrdinal(ExternalJdrDbColumns.DateOfBirth);
        var ordSex = reader.GetOrdinal(ExternalJdrDbColumns.Sex);
        var ordDeclarationAcceptedDate = reader.GetOrdinal(ExternalJdrDbColumns.DeclarationAcceptedDate);
        var ordDisability = reader.GetOrdinal(ExternalJdrDbColumns.Disability);
        var ordOtherDisabilities = reader.GetOrdinal(ExternalJdrDbColumns.OtherDisabilities);
        var ordMedicalCondition = reader.GetOrdinal(ExternalJdrDbColumns.MedicalCondition);
        var ordOtherMedicalCondition = reader.GetOrdinal(ExternalJdrDbColumns.OtherMedicalCondition);
        var ordDementiaHistoryFamily = reader.GetOrdinal(ExternalJdrDbColumns.DementiaHistoryFamily);
        var ordSuffersMemoryProblems = reader.GetOrdinal(ExternalJdrDbColumns.SuffersMemoryProblems);
        var ordEthnicGroup = reader.GetOrdinal(ExternalJdrDbColumns.EthnicGroup);
        var ordRegion = reader.GetOrdinal(ExternalJdrDbColumns.Region);
        var ordDiagnosis = reader.GetOrdinal(ExternalJdrDbColumns.Diagnosis);
        var ordDiagnosisYear = reader.GetOrdinal(ExternalJdrDbColumns.DiagnosisYear);
        var ordDiagnosisMonth = reader.GetOrdinal(ExternalJdrDbColumns.DiagnosisMonth);
        var ordSubtypeDiagnosis = reader.GetOrdinal(ExternalJdrDbColumns.SubtypeDiagnosis);
        var ordSymptomsBeginMonth = reader.GetOrdinal(ExternalJdrDbColumns.SymptomsBeginMonth);
        var ordSymptomsBeginYear = reader.GetOrdinal(ExternalJdrDbColumns.SymptomsBeginYear);
        var ordSymptomsLevel = reader.GetOrdinal(ExternalJdrDbColumns.SymptomsLevel);
        var ordPositiveAmyloidPlaque = reader.GetOrdinal(ExternalJdrDbColumns.PositiveAmyloidPlaque);
        var ordPositiveApoe4 = reader.GetOrdinal(ExternalJdrDbColumns.PositiveApoe4);
        var ordMmseMonth = reader.GetOrdinal(ExternalJdrDbColumns.MmseMonth);
        var ordMmseYear = reader.GetOrdinal(ExternalJdrDbColumns.MmseYear);
        var ordMmseScore = reader.GetOrdinal(ExternalJdrDbColumns.MmseScore);
        var ordAceMonth = reader.GetOrdinal(ExternalJdrDbColumns.AceMonth);
        var ordAceYear = reader.GetOrdinal(ExternalJdrDbColumns.AceYear);
        var ordAceScore = reader.GetOrdinal(ExternalJdrDbColumns.AceScore);
        var ordMocaMonth = reader.GetOrdinal(ExternalJdrDbColumns.MocaMonth);
        var ordMocaYear = reader.GetOrdinal(ExternalJdrDbColumns.MocaYear);
        var ordMocaScore = reader.GetOrdinal(ExternalJdrDbColumns.MocaScore);

        while (await reader.ReadAsync(cancellationToken))
        {
            var volunteerId = reader.GetInt32(ordVolunteerId);
            var dateOfBirth = reader.IsDBNull(ordDateOfBirth) ? (DateTime?)null : reader.GetDateTime(ordDateOfBirth);
            var sex = reader.IsDBNull(ordSex) ? null : reader.GetString(ordSex);
            var declarationAcceptedDate = reader.IsDBNull(ordDeclarationAcceptedDate)
                ? (DateTime?)null
                : reader.GetDateTime(ordDeclarationAcceptedDate);
            var disability = reader.IsDBNull(ordDisability) ? null : reader.GetString(ordDisability);
            var otherDisabilities =
                reader.IsDBNull(ordOtherDisabilities) ? null : reader.GetString(ordOtherDisabilities);
            var medicalCondition = reader.IsDBNull(ordMedicalCondition) ? null : reader.GetString(ordMedicalCondition);
            var otherMedicalCondition = reader.IsDBNull(ordOtherMedicalCondition)
                ? null
                : reader.GetString(ordOtherMedicalCondition);
            var dementiaHistoryFamily = reader.IsDBNull(ordDementiaHistoryFamily)
                ? null
                : reader.GetString(ordDementiaHistoryFamily);
            var suffersMemoryProblems = reader.GetBoolean(ordSuffersMemoryProblems);
            var ethnicGroup = reader.IsDBNull(ordEthnicGroup) ? null : reader.GetString(ordEthnicGroup);
            var region = reader.IsDBNull(ordRegion) ? null : reader.GetString(ordRegion);
            var diagnosis = reader.IsDBNull(ordDiagnosis) ? null : reader.GetString(ordDiagnosis);
            var diagnosisYear = reader.IsDBNull(ordDiagnosisYear) ? (int?)null : reader.GetInt32(ordDiagnosisYear);
            var diagnosisMonth = reader.IsDBNull(ordDiagnosisMonth) ? (int?)null : reader.GetInt32(ordDiagnosisMonth);
            var subtypeDiagnosis = reader.IsDBNull(ordSubtypeDiagnosis) ? null : reader.GetString(ordSubtypeDiagnosis);
            var symptomsMonth = reader.IsDBNull(ordSymptomsBeginMonth)
                ? (int?)null
                : reader.GetInt32(ordSymptomsBeginMonth);
            var symptomsYear = reader.IsDBNull(ordSymptomsBeginYear)
                ? (int?)null
                : reader.GetInt32(ordSymptomsBeginYear);
            var symptomsLevel = reader.IsDBNull(ordSymptomsLevel) ? null : reader.GetString(ordSymptomsLevel);

            var positiveAmyloidPlaque = reader.IsDBNull(ordPositiveAmyloidPlaque)
                ? null
                : reader.GetString(ordPositiveAmyloidPlaque);
            var positiveApoe4 = reader.IsDBNull(ordPositiveApoe4) ? null : reader.GetString(ordPositiveApoe4);
            var mmseMonth = reader.IsDBNull(ordMmseMonth) ? (int?)null : reader.GetInt32(ordMmseMonth);
            var mmseYear = reader.IsDBNull(ordMmseYear) ? (int?)null : reader.GetInt32(ordMmseYear);
            var mmseScore = reader.IsDBNull(ordMmseScore) ? (int?)null : reader.GetInt32(ordMmseScore);
            var aceMonth = reader.IsDBNull(ordAceMonth) ? (int?)null : reader.GetInt32(ordAceMonth);
            var aceYear = reader.IsDBNull(ordAceYear) ? (int?)null : reader.GetInt32(ordAceYear);
            var aceScore = reader.IsDBNull(ordAceScore) ? (int?)null : reader.GetInt32(ordAceScore);
            var mocaMonth = reader.IsDBNull(ordMocaMonth) ? (int?)null : reader.GetInt32(ordMocaMonth);
            var mocaYear = reader.IsDBNull(ordMocaYear) ? (int?)null : reader.GetInt32(ordMocaYear);
            var mocaScore = reader.IsDBNull(ordMocaScore) ? (int?)null : reader.GetInt32(ordMocaScore);

            yield return new Person
            {
                JdrVolunteerId = volunteerId,
                DateOfBirth = dateOfBirth,
                Sex = sex.GetNonEmptyString(),
                Appointment = new Appointment
                {
                    DeclarationAcceptedDate = declarationAcceptedDate,
                    Disabilities = disability.GetNonEmptyString(),
                    OtherDisabilities = otherDisabilities.GetNonEmptyString(),
                    MedicalConditions = medicalCondition.GetNonEmptyString(),
                    OtherMedicalConditions = otherMedicalCondition.GetNonEmptyString(),
                    DementiaHistoryFamily = dementiaHistoryFamily.GetNonEmptyString(),
                    SuffersMemoryProblems = suffersMemoryProblems,
                    EthnicGroup = ethnicGroup.GetNonEmptyString(),
                    Region = region.GetNonEmptyString()
                },
                Diagnosis = new Diagnosis
                {
                    DiagnosisName = diagnosis.GetNonEmptyString(),
                    SubtypeDiagnosisName = subtypeDiagnosis.GetNonEmptyString(),
                    DiagnosisDate = CreateDateFromYearMonth(diagnosisYear, diagnosisMonth),
                    PositiveAmyloidPlaque = positiveAmyloidPlaque.GetNonEmptyString(),
                    PositiveApoe4 = positiveApoe4.GetNonEmptyString()
                },
                Symptom = new Symptom
                {
                    StartDate = CreateDateFromYearMonth(symptomsYear, symptomsMonth),
                    Level = symptomsLevel.GetNonEmptyString()
                },
                Mmse = new Mmse
                {
                    Date = CreateDateFromYearMonth(mmseYear, mmseMonth),
                    Score = mmseScore
                },
                Ace = new Ace
                {
                    Date = CreateDateFromYearMonth(aceYear, aceMonth),
                    Score = aceScore
                },
                Moca = new Moca
                {
                    Date = CreateDateFromYearMonth(mocaYear, mocaMonth),
                    Score = mocaScore
                }
            };
        }
    }

    private static DateTime? CreateDateFromYearMonth(int? year, int? month)
    {
        if (year.HasValue && month is >= 1 and <= 12 &&
            year.Value >= DateTime.MinValue.Year)
        {
            return new DateTime(year.Value, month.Value, 1);
        }

        return null;
    }
}