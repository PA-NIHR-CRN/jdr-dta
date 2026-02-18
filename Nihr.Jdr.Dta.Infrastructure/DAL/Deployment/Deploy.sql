IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    CREATE TABLE [Ace] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NULL,
        [Score] int NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Ace] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    CREATE TABLE [Appointment] (
        [Id] int NOT NULL IDENTITY,
        [DeclarationAcceptedDate] datetime2 NOT NULL,
        [Disabilities] nvarchar(max) NULL,
        [OtherDisabilities] nvarchar(max) NULL,
        [MedicalConditions] nvarchar(max) NULL,
        [OtherMedicalConditions] nvarchar(max) NULL,
        [DementiaHistoryFamily] nvarchar(max) NULL,
        [SuffersMemoryProblems] bit NOT NULL,
        [EthnicGroup] nvarchar(max) NULL,
        [Region] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Appointment] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    CREATE TABLE [Diagnosis] (
        [Id] int NOT NULL IDENTITY,
        [DiagnosisDate] datetime2 NOT NULL,
        [DiagnosisName] nvarchar(max) NULL,
        [SubtypeDiagnosisName] nvarchar(max) NULL,
        [PositiveAmyloidPlaque] nvarchar(max) NULL,
        [PositiveApoe4] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Diagnosis] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    CREATE TABLE [Mmse] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NULL,
        [Score] int NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Mmse] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    CREATE TABLE [Moca] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NULL,
        [Score] int NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Moca] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    CREATE TABLE [Person] (
        [Id] int NOT NULL IDENTITY,
        [JdrVolunteerId] int NOT NULL,
        [DateOfBirth] datetime2 NULL,
        [Sex] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Person] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    CREATE TABLE [Symptom] (
        [Id] int NOT NULL IDENTITY,
        [StartDate] datetime2 NULL,
        [Level] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Symptom] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213142057_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260213142057_InitialCreate', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Symptom] ADD [PersonId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Moca] ADD [PersonId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Mmse] ADD [PersonId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Diagnosis] ADD [PersonId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Appointment] ADD [PersonId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Ace] ADD [PersonId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Symptom_PersonId] ON [Symptom] ([PersonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Moca_PersonId] ON [Moca] ([PersonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Mmse_PersonId] ON [Mmse] ([PersonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Diagnosis_PersonId] ON [Diagnosis] ([PersonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Appointment_PersonId] ON [Appointment] ([PersonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Ace_PersonId] ON [Ace] ([PersonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Ace] ADD CONSTRAINT [FK_Ace_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Person] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Appointment] ADD CONSTRAINT [FK_Appointment_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Person] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Diagnosis] ADD CONSTRAINT [FK_Diagnosis_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Person] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Mmse] ADD CONSTRAINT [FK_Mmse_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Person] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Moca] ADD CONSTRAINT [FK_Moca_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Person] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    ALTER TABLE [Symptom] ADD CONSTRAINT [FK_Symptom_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Person] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260213173500_AddPersonFk'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260213173500_AddPersonFk', N'8.0.24');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260217183435_UpdateNullableFields'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Diagnosis]') AND [c].[name] = N'DiagnosisDate');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Diagnosis] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Diagnosis] ALTER COLUMN [DiagnosisDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260217183435_UpdateNullableFields'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointment]') AND [c].[name] = N'DeclarationAcceptedDate');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Appointment] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Appointment] ALTER COLUMN [DeclarationAcceptedDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260217183435_UpdateNullableFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260217183435_UpdateNullableFields', N'8.0.24');
END;
GO

COMMIT;
GO

