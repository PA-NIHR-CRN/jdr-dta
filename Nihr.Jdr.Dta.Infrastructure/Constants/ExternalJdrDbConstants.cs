namespace Nihr.Jdr.Dta.Infrastructure.Constants;

public static class ExternalJdrDbColumns
{
    public const string VolunteerId = "VolunteerId";
    public const string DateOfBirth = "DateOfBirth";
    public const string Sex = "Sex";
    public const string DeclarationAcceptedDate = "DeclarationAcceptedDate";
    public const string Disability = "Disability";
    public const string OtherDisabilities = "OtherDisabilities";
    public const string MedicalCondition = "MedicalCondition";
    public const string OtherMedicalCondition = "OtherMedicalCondition";
    public const string DementiaHistoryFamily = "DementiaHistoryFamily";
    public const string SuffersMemoryProblems = "SuffersMemoryProblems";
    public const string EthnicGroup = "EthnicGroup";
    public const string Region = "Region";
    public const string Diagnosis = "Diagnosis";
    public const string DiagnosisYear = "DiagnosisYear";
    public const string DiagnosisMonth = "DiagnosisMonth";
    public const string SubtypeDiagnosis = "SubtypeDiagnosis";
    public const string PositiveAmyloidPlaque = "PositiveAmyloidPlaque";
    public const string PositiveApoe4 = "PositiveApoe4";
    public const string MmseMonth = "MmseMonth";
    public const string MmseYear = "MmseYear";
    public const string MmseScore = "MmseScore";
    public const string AceMonth = "AceMonth";
    public const string AceYear = "AceYear";
    public const string AceScore = "AceScore";
    public const string MocaMonth = "MocaMonth";
    public const string MocaYear = "MocaYear";
    public const string MocaScore = "MocaScore";
    public const string SymptomsBeginMonth = "SymptomsBeginMonth";
    public const string SymptomsBeginYear = "SymptomsBeginYear";
    public const string SymptomsLevel = "SymptomsLevel";
}

public static class ExternalJdrDbQueries
{
    public const string GetVolunteers = """
                                        SELECT
                                            v.id AS 'VolunteerId',
                                            p.dateofbirth AS 'DateOfBirth',
                                            s.en AS 'Sex',
                                            d.declaration_accepted_date AS 'DeclarationAcceptedDate',
                                            GROUP_CONCAT(DISTINCT dis.en SEPARATOR ', ') AS 'Disability',
                                            m.other_disabilities AS 'OtherDisabilities',
                                            GROUP_CONCAT(DISTINCT con.en SEPARATOR ', ') AS 'MedicalCondition',
                                            m.other_medical_condition AS 'OtherMedicalCondition',
                                            d.dementia_history_family AS 'DementiaHistoryFamily',
                                            d.suffers_memory_problems AS 'SuffersMemoryProblems',
                                            d.ethnic_group AS 'EthnicGroup',
                                            r.en AS 'Region',
                                            GROUP_CONCAT(DISTINCT dia.en SEPARATOR ', ') AS 'Diagnosis',
                                            m.volunteer_diagnosis_date_year AS 'DiagnosisYear',
                                            m.volunteer_diagnosis_date_month AS 'DiagnosisMonth',
                                            m.subtype_diagnosis AS 'SubtypeDiagnosis',
                                            m.symptoms_begin_date_month 'SymptomsBeginMonth',
                                            m.symptoms_begin_date_year 'SymptomsBeginYear',
                                            m.symptoms_level 'SymptomsLevel',
                                            m.positive_amyloid_plaque AS 'PositiveAmyloidPlaque',
                                            m.positive_apoe4 AS 'PositiveApoe4',
                                            m.mmse_date_month AS 'MmseMonth',
                                            m.mmse_date_year AS 'MmseYear',
                                            m.mmse_score AS 'MmseScore',
                                            m.ace_date_month AS 'AceMonth',
                                            m.ace_date_year AS 'AceYear',
                                            m.ace_score AS 'AceScore',
                                            m.moca_date_month AS 'MocaMonth',
                                            m.moca_date_year AS 'MocaYear',
                                            m.moca_score AS 'MocaScore'
                                        FROM
                                            `volunteer` v
                                            INNER JOIN `person` p ON p.id = v.person_id
                                            INNER JOIN `sex` s ON s.id = p.sex_id
                                            INNER JOIN `demographic` d ON d.volunteer_id = v.id
                                            LEFT JOIN `region` r ON r.id = d.region_id
                                            INNER JOIN `medical` m ON m.volunteer_id = v.id
                                            LEFT JOIN `medical_diagnosis` med_dia ON med_dia.medical_id = m.id
                                            LEFT JOIN `diagnosis` dia ON dia.id = med_dia.diagnosis_id
                                            LEFT JOIN `medical_disability` med_dis ON med_dis.medical_id = m.id
                                            LEFT JOIN `disabilities` dis ON dis.id = med_dis.disability_id
                                            LEFT JOIN `medical_medical_conditions` med_con ON med_con.medical_id = m.id
                                            LEFT JOIN `medical_conditions` con ON con.id = med_con.medical_condition_id
                                        WHERE 
                                            v.is_soft_deleted = 0
                                        GROUP BY
                                            v.id
                                        ORDER BY
                                            v.id;
                                        """;
}