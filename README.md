# JDR DTA

`jdr-dta` is a .NET data pipeline that synchronises JDR source data into a staging database, runs a Carrot CDM transform, and bulk-loads OMOP TSV outputs into a target schema.

## What The Job Does

When run with no CLI arguments, the executable performs two stages:

1. Stream source records from the external JDR database, then upsert to staging in batches.
2. Export staging source tables to CSV, run `carrot-transform`, then import generated OMOP TSV files.

If stage 1 fails, stage 2 is skipped.

## Solution Structure

- `Nihr.Jdr.Dta.Job`: application entry point, DI/configuration, orchestration, utilities.
- `Nihr.Jdr.Dta.Domain`: domain entities and interfaces.
- `Nihr.Jdr.Dta.Infrastructure`: repository implementations, EF context/migrations, external integrations.
- `CarrotCDM/`: Carrot rules/config/input/output assets used by transform workflows.

## Prerequisites

- .NET SDK 10 (project targets `net10.0`).
- Access to the required SQL Server/MySQL/GCP/AWS resources for your environment.
- For local non-container Carrot runs: Python 3 and `carrot-transform` installed, or set `CarrotCdm:PythonPath` in Development.

The included Dockerfile installs `carrot-transform` into `/opt/carrot-venv` for container execution.

## Configuration

Application settings are loaded from standard .NET configuration sources (for example `appsettings.json`, environment variables, and secret providers).

For local development, create `Nihr.Jdr.Dta.Job/appsettings.user.json` based on `Nihr.Jdr.Dta.Job/appsettings.json` and provide the same key structure with your local values. Do not commit `appsettings.user.json` to source control.

In deployed environments, secrets are provided by AWS Secrets Manager.

Required settings used by startup/runtime:

- `ConnectionStrings:DefaultConnection`
- `JdrDbSettings:IpAddress`
- `JdrDbSettings:Username`
- `JdrDbSettings:DatabaseName`
- `JdrDbSettings:JdrGcpSettings:ServiceAccountKey`
- `JdrDbSettings:JdrGcpSettings:FullyQualifiedSecretName`
- `CarrotCdm:InputDirectory`
- `CarrotCdm:RulesFile`
- `CarrotCdm:PersonTable`
- `CarrotCdm:OutputDirectory`
- `CarrotCdm:DdlFile`
- `CarrotCdm:ConfigFile`
- `CarrotCdm:SourceExport:ConnectionStringName`
- `CarrotCdm:SourceExport:Tables`
- `Omop:ConnectionString`
- `Omop:SchemaName`

Optional:

- `CarrotCdm:PythonPath` (used in Development to override the default python path).
- `Omop:SkipImportTableNames` (tables to skip when importing TSV outputs).

## Running The Job

Run full pipeline (default behavior, no args):

```bash
dotnet run --project Nihr.Jdr.Dta.Job
```

Available utility commands:

```bash
dotnet run --project Nihr.Jdr.Dta.Job -- --deploy-ddl
dotnet run --project Nihr.Jdr.Dta.Job -- --load-ref-data s3://your-bucket/path/to/vocabularies/
```

Supported vocabulary files for `--load-ref-data`:

- `CONCEPT.csv`
- `VOCABULARY.csv`
- `DOMAIN.csv`
- `CONCEPT_CLASS.csv`
- `RELATIONSHIP.csv`
- `CONCEPT_RELATIONSHIP.csv`
- `CONCEPT_SYNONYM.csv`
- `CONCEPT_ANCESTOR.csv`
- `DRUG_STRENGTH.csv`

## EF Core Migrations

Use startup project `Nihr.Jdr.Dta.Job` and migrations project `Nihr.Jdr.Dta.Infrastructure`.

Add migration:

```bash
dotnet ef migrations add --project Nihr.Jdr.Dta.Infrastructure/Nihr.Jdr.Dta.Infrastructure.csproj --startup-project Nihr.Jdr.Dta.Job/Nihr.Jdr.Dta.Job.csproj --context Nihr.Jdr.Dta.Infrastructure.DAL.JdrDtaDbContext --configuration Debug <MigrationName> --output-dir DAL/Migrations
```

Generate idempotent deployment script:

```bash
dotnet ef migrations script --idempotent --output ./Nihr.Jdr.Dta.Infrastructure/DAL/Deployment/Deploy.sql --project Nihr.Jdr.Dta.Infrastructure --startup-project Nihr.Jdr.Dta.Job
```

