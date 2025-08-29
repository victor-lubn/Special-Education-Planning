/*
Script de implementación para snapshot_v0.0.0

Una herramienta generó este código.
Los cambios realizados en este archivo podrían generar un comportamiento incorrecto y se perderán si
se vuelve a generar el código.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "snapshot_v0.0.0"
:setvar DefaultFilePrefix "snapshot_v0.0.0"
:setvar DefaultDataPath "C:\Users\jomartim\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\"
:setvar DefaultLogPath "C:\Users\jomartim\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\"

GO
:on error exit
GO
/*
Detectar el modo SQLCMD y deshabilitar la ejecución del script si no se admite el modo SQLCMD.
Para volver a habilitar el script después de habilitar el modo SQLCMD, ejecute lo siguiente:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'El modo SQLCMD debe estar habilitado para ejecutar correctamente este script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
/*
Se está quitando la columna [dbo].[Plan].[Catalogue]; puede que se pierdan datos.

Se está quitando la columna [dbo].[Plan].[SaleVersionId]; puede que se pierdan datos.

Debe agregarse la columna [dbo].[Plan].[CatalogId] de la tabla [dbo].[Plan], pero esta columna no tiene un valor predeterminado y no admite valores NULL. Si la tabla contiene datos, el script ALTER no funcionará. Para evitar este problema, agregue un valor predeterminado a la columna, márquela de modo que permita valores NULL o habilite la generación de valores predeterminados inteligentes como opción de implementación.

Debe agregarse la columna [dbo].[Plan].[MasterVersionId] de la tabla [dbo].[Plan], pero esta columna no tiene un valor predeterminado y no admite valores NULL. Si la tabla contiene datos, el script ALTER no funcionará. Para evitar este problema, agregue un valor predeterminado a la columna, márquela de modo que permita valores NULL o habilite la generación de valores predeterminados inteligentes como opción de implementación.
*/

IF EXISTS (select top 1 1 from [dbo].[Plan])
    RAISERROR (N'Se detectaron filas. La actualización del esquema va a terminar debido a una posible pérdida de datos.', 16, 127) WITH NOWAIT

GO
/*
Se está quitando la columna [dbo].[RomItem].[BuilderId]; puede que se pierdan datos.

Se está quitando la columna [dbo].[RomItem].[Quantity]; puede que se pierdan datos.

Se está quitando la columna [dbo].[RomItem].[VersionId]; puede que se pierdan datos.

Debe agregarse la columna [dbo].[RomItem].[ItemName] de la tabla [dbo].[RomItem], pero esta columna no tiene un valor predeterminado y no admite valores NULL. Si la tabla contiene datos, el script ALTER no funcionará. Para evitar este problema, agregue un valor predeterminado a la columna, márquela de modo que permita valores NULL o habilite la generación de valores predeterminados inteligentes como opción de implementación.

Debe agregarse la columna [dbo].[RomItem].[Qty] de la tabla [dbo].[RomItem], pero esta columna no tiene un valor predeterminado y no admite valores NULL. Si la tabla contiene datos, el script ALTER no funcionará. Para evitar este problema, agregue un valor predeterminado a la columna, márquela de modo que permita valores NULL o habilite la generación de valores predeterminados inteligentes como opción de implementación.
*/

IF EXISTS (select top 1 1 from [dbo].[RomItem])
    RAISERROR (N'Se detectaron filas. La actualización del esquema va a terminar debido a una posible pérdida de datos.', 16, 127) WITH NOWAIT

GO
/*
Se está quitando la columna [dbo].[Version].[Catalog]; puede que se pierdan datos.

Se está quitando la columna [dbo].[Version].[SalePlanId]; puede que se pierdan datos.

Debe agregarse la columna [dbo].[Version].[CatalogId] de la tabla [dbo].[Version], pero esta columna no tiene un valor predeterminado y no admite valores NULL. Si la tabla contiene datos, el script ALTER no funcionará. Para evitar este problema, agregue un valor predeterminado a la columna, márquela de modo que permita valores NULL o habilite la generación de valores predeterminados inteligentes como opción de implementación.
*/

IF EXISTS (select top 1 1 from [dbo].[Version])
    RAISERROR (N'Se detectaron filas. La actualización del esquema va a terminar debido a una posible pérdida de datos.', 16, 127) WITH NOWAIT

GO
PRINT N'Quitando [dbo].[RomItem].[IX_RomItem_VersionId]...';


GO
DROP INDEX [IX_RomItem_VersionId]
    ON [dbo].[RomItem];


GO
PRINT N'Quitando [dbo].[Version].[IX_Version_SalePlanId]...';


GO
DROP INDEX [IX_Version_SalePlanId]
    ON [dbo].[Version];


GO
PRINT N'Quitando [dbo].[FK_Plan_Project_ProjectId]...';


GO
ALTER TABLE [dbo].[Plan] DROP CONSTRAINT [FK_Plan_Project_ProjectId];


GO
PRINT N'Quitando [dbo].[FK_Plan_User_BuilderId]...';


GO
ALTER TABLE [dbo].[Plan] DROP CONSTRAINT [FK_Plan_User_BuilderId];


GO
PRINT N'Quitando [dbo].[FK_Plan_User_EducationerId]...';


GO
ALTER TABLE [dbo].[Plan] DROP CONSTRAINT [FK_Plan_User_EducationerId];


GO
PRINT N'Quitando [dbo].[FK_Plan_User_EndUserId]...';


GO
ALTER TABLE [dbo].[Plan] DROP CONSTRAINT [FK_Plan_User_EndUserId];


GO
PRINT N'Quitando [dbo].[FK_PlanGroup_Plan_PlanId]...';


GO
ALTER TABLE [dbo].[PlanGroup] DROP CONSTRAINT [FK_PlanGroup_Plan_PlanId];


GO
PRINT N'Quitando [dbo].[FK_Version_Plan_PlanId]...';


GO
ALTER TABLE [dbo].[Version] DROP CONSTRAINT [FK_Version_Plan_PlanId];


GO
PRINT N'Quitando [dbo].[FK_Version_Plan_SalePlanId]...';


GO
ALTER TABLE [dbo].[Version] DROP CONSTRAINT [FK_Version_Plan_SalePlanId];


GO
PRINT N'Quitando [dbo].[FK_RomItem_Version_VersionId]...';


GO
ALTER TABLE [dbo].[RomItem] DROP CONSTRAINT [FK_RomItem_Version_VersionId];


GO
PRINT N'Quitando [dbo].[FK_BuilderEducationerAiep_User_BuilderId]...';


GO
ALTER TABLE [dbo].[BuilderEducationerAiep] DROP CONSTRAINT [FK_BuilderEducationerAiep_User_BuilderId];


GO
PRINT N'Quitando [dbo].[FK_BuilderEducationerAiep_User_EducationerId]...';


GO
ALTER TABLE [dbo].[BuilderEducationerAiep] DROP CONSTRAINT [FK_BuilderEducationerAiep_User_EducationerId];


GO
PRINT N'Quitando [dbo].[FK_Aiep_User_ManagerId]...';


GO
ALTER TABLE [dbo].[Aiep] DROP CONSTRAINT [FK_Aiep_User_ManagerId];


GO
PRINT N'Quitando [dbo].[FK_GroupUser_User_UserId]...';


GO
ALTER TABLE [dbo].[GroupUser] DROP CONSTRAINT [FK_GroupUser_User_UserId];


GO
PRINT N'Quitando [dbo].[FK_User_ReleaseInfo_ReleaseInfoId]...';


GO
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_ReleaseInfo_ReleaseInfoId];


GO
PRINT N'Iniciando recompilación de la tabla [dbo].[AuditLog]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_AuditLog] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Content]       NVARCHAR (MAX) NULL,
    [DateTime]      DATETIME2 (7)  NOT NULL,
    [HttpQuery]     NVARCHAR (MAX) NULL,
    [LocalAddress]  NVARCHAR (MAX) NULL,
    [RemoteAddress] NVARCHAR (MAX) NULL,
    [User]          NVARCHAR (MAX) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_AuditLog1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[AuditLog])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_AuditLog] ON;
        INSERT INTO [dbo].[tmp_ms_xx_AuditLog] ([Id], [DateTime], [HttpQuery], [LocalAddress], [RemoteAddress], [User])
        SELECT   [Id],
                 [DateTime],
                 [HttpQuery],
                 [LocalAddress],
                 [RemoteAddress],
                 [User]
        FROM     [dbo].[AuditLog]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_AuditLog] OFF;
    END

DROP TABLE [dbo].[AuditLog];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_AuditLog]', N'AuditLog';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_AuditLog1]', N'PK_AuditLog', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Iniciando recompilación de la tabla [dbo].[Plan]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Plan] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [BuilderId]           INT            NULL,
    [CatalogId]           INT            NOT NULL,
    [CreationDate]        DATETIME2 (7)  NOT NULL,
    [DeletedDate]         DATETIME2 (7)  NULL,
    [DeletedUser]         NVARCHAR (MAX) NULL,
    [EducationerId]          INT            NULL,
    [EndUserAddress0]     NVARCHAR (MAX) NULL,
    [EndUserComment]      NVARCHAR (MAX) NULL,
    [EndUserFirstName]    NVARCHAR (MAX) NULL,
    [EndUserFullName]     NVARCHAR (MAX) NULL,
    [EndUserId]           INT            NULL,
    [EndUserMobileNumber] NVARCHAR (MAX) NULL,
    [EndUserPostcode]     NVARCHAR (MAX) NULL,
    [EndUserSurname]      NVARCHAR (MAX) NULL,
    [IsDeleted]           BIT            NOT NULL,
    [IsStarred]           BIT            NOT NULL,
    [KeyName]             NVARCHAR (MAX) NULL,
    [LastModify]          DATETIME2 (7)  NOT NULL,
    [LastOpen]            DATETIME2 (7)  NOT NULL,
    [MasterVersionId]     INT            NOT NULL,
    [PlanCode]            NVARCHAR (MAX) NULL,
    [PlanState]           INT            NOT NULL,
    [ProjectId]           INT            NOT NULL,
    [Survey]              BIT            NOT NULL,
    [Title]               NVARCHAR (MAX) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Plan1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Plan])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Plan] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Plan] ([Id], [BuilderId], [CreationDate], [DeletedDate], [DeletedUser], [EducationerId], [EndUserAddress0], [EndUserFirstName], [EndUserFullName], [EndUserId], [EndUserMobileNumber], [EndUserPostcode], [EndUserSurname], [IsDeleted], [IsStarred], [KeyName], [LastModify], [LastOpen], [PlanCode], [PlanState], [ProjectId], [Survey], [Title])
        SELECT   [Id],
                 [BuilderId],
                 [CreationDate],
                 [DeletedDate],
                 [DeletedUser],
                 [EducationerId],
                 [EndUserAddress0],
                 [EndUserFirstName],
                 [EndUserFullName],
                 [EndUserId],
                 [EndUserMobileNumber],
                 [EndUserPostcode],
                 [EndUserSurname],
                 [IsDeleted],
                 [IsStarred],
                 [KeyName],
                 [LastModify],
                 [LastOpen],
                 [PlanCode],
                 [PlanState],
                 [ProjectId],
                 [Survey],
                 [Title]
        FROM     [dbo].[Plan]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Plan] OFF;
    END

DROP TABLE [dbo].[Plan];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Plan]', N'Plan';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Plan1]', N'PK_Plan', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creando [dbo].[Plan].[IX_Plan_ProjectId]...';


GO
CREATE NONCLUSTERED INDEX [IX_Plan_ProjectId]
    ON [dbo].[Plan]([ProjectId] ASC);


GO
PRINT N'Creando [dbo].[Plan].[IX_Plan_EndUserId]...';


GO
CREATE NONCLUSTERED INDEX [IX_Plan_EndUserId]
    ON [dbo].[Plan]([EndUserId] ASC);


GO
PRINT N'Creando [dbo].[Plan].[IX_Plan_EducationerId]...';


GO
CREATE NONCLUSTERED INDEX [IX_Plan_EducationerId]
    ON [dbo].[Plan]([EducationerId] ASC);


GO
PRINT N'Creando [dbo].[Plan].[IX_Plan_BuilderId]...';


GO
CREATE NONCLUSTERED INDEX [IX_Plan_BuilderId]
    ON [dbo].[Plan]([BuilderId] ASC);


GO
PRINT N'Iniciando recompilación de la tabla [dbo].[RomItem]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_RomItem] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [CatalogId]    INT            NULL,
    [Colour]       NVARCHAR (MAX) NULL,
    [ItemName]     NVARCHAR (MAX) NOT NULL,
    [Qty]          INT            NOT NULL,
    [Range]        NVARCHAR (MAX) NULL,
    [SerialNumber] NVARCHAR (MAX) NULL,
    [Sku]          NVARCHAR (MAX) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_RomItem1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[RomItem])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_RomItem] ON;
        INSERT INTO [dbo].[tmp_ms_xx_RomItem] ([Id], [Colour], [Range], [SerialNumber], [Sku])
        SELECT   [Id],
                 [Colour],
                 [Range],
                 [SerialNumber],
                 [Sku]
        FROM     [dbo].[RomItem]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_RomItem] OFF;
    END

DROP TABLE [dbo].[RomItem];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_RomItem]', N'RomItem';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_RomItem1]', N'PK_RomItem', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creando [dbo].[RomItem].[IX_RomItem_CatalogId]...';


GO
CREATE NONCLUSTERED INDEX [IX_RomItem_CatalogId]
    ON [dbo].[RomItem]([CatalogId] ASC);


GO
PRINT N'Iniciando recompilación de la tabla [dbo].[User]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_User] (
    [AccountEmail]              NVARCHAR (MAX) NULL,
    [AccountNumber]             NVARCHAR (MAX) NULL,
    [CompanyName]               NVARCHAR (MAX) NULL,
    [CountryCodeLandLineNumber] INT            NULL,
    [CountryCodeMobileNumber]   INT            NULL,
    [CreationDate]              DATETIME2 (7)  NULL,
    [EducationerName]              NVARCHAR (MAX) NULL,
    [FullName]                  NVARCHAR (MAX) NULL,
    [HouseNameorNumber]         NVARCHAR (MAX) NULL,
    [aiepurvey]              BIT            NULL,
    [Initials]                  NVARCHAR (MAX) NULL,
    [LastModify]                DATETIME2 (7)  NULL,
    [PlotReference]             NVARCHAR (MAX) NULL,
    [ReleaseInfoId]             INT            NULL,
    [Comment]                   NVARCHAR (MAX) NULL,
    [CountryCode]               INT            NULL,
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Address0]                  NVARCHAR (MAX) NULL,
    [Address1]                  NVARCHAR (MAX) NULL,
    [Address2]                  NVARCHAR (MAX) NULL,
    [Address3]                  NVARCHAR (MAX) NULL,
    [Address4]                  NVARCHAR (MAX) NULL,
    [Address5]                  NVARCHAR (MAX) NULL,
    [ContactEmail]              NVARCHAR (MAX) NULL,
    [DeletedDate]               DATETIME2 (7)  NULL,
    [DeletedUser]               NVARCHAR (MAX) NULL,
    [Discriminator]             NVARCHAR (MAX) NOT NULL,
    [FirstName]                 NVARCHAR (20)  NULL,
    [IsDeleted]                 BIT            NOT NULL,
    [LandLineNumber]            NVARCHAR (MAX) NULL,
    [MobileNumber]              NVARCHAR (MAX) NULL,
    [Postcode]                  NVARCHAR (8)   NULL,
    [Surname]                   NVARCHAR (20)  NOT NULL,
    [Title]                     INT            NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_User1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[User])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_User] ON;
        INSERT INTO [dbo].[tmp_ms_xx_User] ([Id], [AccountEmail], [AccountNumber], [CompanyName], [ContactEmail], [CountryCodeLandLineNumber], [CountryCodeMobileNumber], [CreationDate], [EducationerName], [FullName], [HouseNameorNumber], [aiepurvey], [Initials], [LandLineNumber], [LastModify], [PlotReference], [ReleaseInfoId], [CountryCode], [Address0], [Address1], [Address2], [Address3], [Address4], [Address5], [DeletedDate], [DeletedUser], [Discriminator], [FirstName], [IsDeleted], [MobileNumber], [Postcode], [Surname], [Title])
        SELECT   [Id],
                 [AccountEmail],
                 [AccountNumber],
                 [CompanyName],
                 [ContactEmail],
                 [CountryCodeLandLineNumber],
                 [CountryCodeMobileNumber],
                 [CreationDate],
                 [EducationerName],
                 [FullName],
                 [HouseNameorNumber],
                 [aiepurvey],
                 [Initials],
                 [LandLineNumber],
                 [LastModify],
                 [PlotReference],
                 [ReleaseInfoId],
                 [CountryCode],
                 [Address0],
                 [Address1],
                 [Address2],
                 [Address3],
                 [Address4],
                 [Address5],
                 [DeletedDate],
                 [DeletedUser],
                 [Discriminator],
                 [FirstName],
                 [IsDeleted],
                 [MobileNumber],
                 [Postcode],
                 [Surname],
                 [Title]
        FROM     [dbo].[User]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_User] OFF;
    END

DROP TABLE [dbo].[User];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_User]', N'User';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_User1]', N'PK_User', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creando [dbo].[User].[IX_User_ReleaseInfoId]...';


GO
CREATE NONCLUSTERED INDEX [IX_User_ReleaseInfoId]
    ON [dbo].[User]([ReleaseInfoId] ASC);


GO
PRINT N'Iniciando recompilación de la tabla [dbo].[Version]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Version] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [CatalogId]     INT            NOT NULL,
    [Creation]      DATETIME2 (7)  NOT NULL,
    [KeyName]       NVARCHAR (MAX) NULL,
    [LastModify]    DATETIME2 (7)  NOT NULL,
    [MasterPlanId]  INT            NULL,
    [PlanId]        INT            NOT NULL,
    [Preview]       NVARCHAR (MAX) NULL,
    [PreviewPath]   NVARCHAR (MAX) NULL,
    [Range]         NVARCHAR (MAX) NULL,
    [Rom]           NVARCHAR (MAX) NULL,
    [RomPath]       NVARCHAR (MAX) NULL,
    [VersionNotes]  NVARCHAR (MAX) NULL,
    [VersionNumber] INT            NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Version1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Version])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Version] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Version] ([Id], [Creation], [KeyName], [LastModify], [PlanId], [Preview], [PreviewPath], [Range], [Rom], [RomPath], [VersionNotes], [VersionNumber])
        SELECT   [Id],
                 [Creation],
                 [KeyName],
                 [LastModify],
                 [PlanId],
                 [Preview],
                 [PreviewPath],
                 [Range],
                 [Rom],
                 [RomPath],
                 [VersionNotes],
                 [VersionNumber]
        FROM     [dbo].[Version]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Version] OFF;
    END

DROP TABLE [dbo].[Version];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Version]', N'Version';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Version1]', N'PK_Version', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creando [dbo].[Version].[IX_Version_PlanId]...';


GO
CREATE NONCLUSTERED INDEX [IX_Version_PlanId]
    ON [dbo].[Version]([PlanId] ASC);


GO
PRINT N'Creando [dbo].[Version].[IX_Version_CatalogId]...';


GO
CREATE NONCLUSTERED INDEX [IX_Version_CatalogId]
    ON [dbo].[Version]([CatalogId] ASC);


GO
PRINT N'Creando [dbo].[Version].[IX_Version_MasterPlanId]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Version_MasterPlanId]
    ON [dbo].[Version]([MasterPlanId] ASC) WHERE ([MasterPlanId] IS NOT NULL);


GO
PRINT N'Creando [dbo].[Catalog]...';


GO
CREATE TABLE [dbo].[Catalog] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Code]    NVARCHAR (MAX) NULL,
    [Enabled] BIT            NOT NULL,
    [Name]    NVARCHAR (MAX) NULL,
    [Range]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Catalog] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creando [dbo].[VersionRomItem]...';


GO
CREATE TABLE [dbo].[VersionRomItem] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [ItemName]  NVARCHAR (MAX) NOT NULL,
    [Quantity]  INT            NOT NULL,
    [RomItemId] INT            NOT NULL,
    [VersionId] INT            NOT NULL,
    CONSTRAINT [PK_VersionRomItem] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creando [dbo].[VersionRomItem].[IX_VersionRomItem_VersionId]...';


GO
CREATE NONCLUSTERED INDEX [IX_VersionRomItem_VersionId]
    ON [dbo].[VersionRomItem]([VersionId] ASC);


GO
PRINT N'Creando [dbo].[VersionRomItem].[IX_VersionRomItem_RomItemId]...';


GO
CREATE NONCLUSTERED INDEX [IX_VersionRomItem_RomItemId]
    ON [dbo].[VersionRomItem]([RomItemId] ASC);


GO
PRINT N'Modificando [dbo].[PlanCodeSequence]...';


GO
ALTER SEQUENCE [dbo].[PlanCodeSequence]
    RESTART WITH 1;


GO
PRINT N'Creando [dbo].[FK_Plan_Project_ProjectId]...';


GO
ALTER TABLE [dbo].[Plan] WITH NOCHECK
    ADD CONSTRAINT [FK_Plan_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_Plan_User_BuilderId]...';


GO
ALTER TABLE [dbo].[Plan] WITH NOCHECK
    ADD CONSTRAINT [FK_Plan_User_BuilderId] FOREIGN KEY ([BuilderId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creando [dbo].[FK_Plan_User_EducationerId]...';


GO
ALTER TABLE [dbo].[Plan] WITH NOCHECK
    ADD CONSTRAINT [FK_Plan_User_EducationerId] FOREIGN KEY ([EducationerId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creando [dbo].[FK_Plan_User_EndUserId]...';


GO
ALTER TABLE [dbo].[Plan] WITH NOCHECK
    ADD CONSTRAINT [FK_Plan_User_EndUserId] FOREIGN KEY ([EndUserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creando [dbo].[FK_PlanGroup_Plan_PlanId]...';


GO
ALTER TABLE [dbo].[PlanGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_PlanGroup_Plan_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plan] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_Version_Plan_PlanId]...';


GO
ALTER TABLE [dbo].[Version] WITH NOCHECK
    ADD CONSTRAINT [FK_Version_Plan_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plan] ([Id]);


GO
PRINT N'Creando [dbo].[FK_RomItem_Catalog_CatalogId]...';


GO
ALTER TABLE [dbo].[RomItem] WITH NOCHECK
    ADD CONSTRAINT [FK_RomItem_Catalog_CatalogId] FOREIGN KEY ([CatalogId]) REFERENCES [dbo].[Catalog] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_BuilderEducationerAiep_User_BuilderId]...';


GO
ALTER TABLE [dbo].[BuilderEducationerAiep] WITH NOCHECK
    ADD CONSTRAINT [FK_BuilderEducationerAiep_User_BuilderId] FOREIGN KEY ([BuilderId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_BuilderEducationerAiep_User_EducationerId]...';


GO
ALTER TABLE [dbo].[BuilderEducationerAiep] WITH NOCHECK
    ADD CONSTRAINT [FK_BuilderEducationerAiep_User_EducationerId] FOREIGN KEY ([EducationerId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creando [dbo].[FK_Aiep_User_ManagerId]...';


GO
ALTER TABLE [dbo].[Aiep] WITH NOCHECK
    ADD CONSTRAINT [FK_Aiep_User_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_GroupUser_User_UserId]...';


GO
ALTER TABLE [dbo].[GroupUser] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupUser_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_User_ReleaseInfo_ReleaseInfoId]...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_ReleaseInfo_ReleaseInfoId] FOREIGN KEY ([ReleaseInfoId]) REFERENCES [dbo].[ReleaseInfo] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_Version_Catalog_CatalogId]...';


GO
ALTER TABLE [dbo].[Version] WITH NOCHECK
    ADD CONSTRAINT [FK_Version_Catalog_CatalogId] FOREIGN KEY ([CatalogId]) REFERENCES [dbo].[Catalog] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creando [dbo].[FK_Version_Plan_MasterPlanId]...';


GO
ALTER TABLE [dbo].[Version] WITH NOCHECK
    ADD CONSTRAINT [FK_Version_Plan_MasterPlanId] FOREIGN KEY ([MasterPlanId]) REFERENCES [dbo].[Plan] ([Id]);


GO
PRINT N'Creando [dbo].[FK_VersionRomItem_RomItem_RomItemId]...';


GO
ALTER TABLE [dbo].[VersionRomItem] WITH NOCHECK
    ADD CONSTRAINT [FK_VersionRomItem_RomItem_RomItemId] FOREIGN KEY ([RomItemId]) REFERENCES [dbo].[RomItem] ([Id]);


GO
PRINT N'Creando [dbo].[FK_VersionRomItem_Version_VersionId]...';


GO
ALTER TABLE [dbo].[VersionRomItem] WITH NOCHECK
    ADD CONSTRAINT [FK_VersionRomItem_Version_VersionId] FOREIGN KEY ([VersionId]) REFERENCES [dbo].[Version] ([Id]);


GO
PRINT N'Comprobando los datos existentes con las restricciones recién creadas';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Plan] WITH CHECK CHECK CONSTRAINT [FK_Plan_Project_ProjectId];

ALTER TABLE [dbo].[Plan] WITH CHECK CHECK CONSTRAINT [FK_Plan_User_BuilderId];

ALTER TABLE [dbo].[Plan] WITH CHECK CHECK CONSTRAINT [FK_Plan_User_EducationerId];

ALTER TABLE [dbo].[Plan] WITH CHECK CHECK CONSTRAINT [FK_Plan_User_EndUserId];

ALTER TABLE [dbo].[PlanGroup] WITH CHECK CHECK CONSTRAINT [FK_PlanGroup_Plan_PlanId];

ALTER TABLE [dbo].[Version] WITH CHECK CHECK CONSTRAINT [FK_Version_Plan_PlanId];

ALTER TABLE [dbo].[RomItem] WITH CHECK CHECK CONSTRAINT [FK_RomItem_Catalog_CatalogId];

ALTER TABLE [dbo].[BuilderEducationerAiep] WITH CHECK CHECK CONSTRAINT [FK_BuilderEducationerAiep_User_BuilderId];

ALTER TABLE [dbo].[BuilderEducationerAiep] WITH CHECK CHECK CONSTRAINT [FK_BuilderEducationerAiep_User_EducationerId];

ALTER TABLE [dbo].[Aiep] WITH CHECK CHECK CONSTRAINT [FK_Aiep_User_ManagerId];

ALTER TABLE [dbo].[GroupUser] WITH CHECK CHECK CONSTRAINT [FK_GroupUser_User_UserId];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_ReleaseInfo_ReleaseInfoId];

ALTER TABLE [dbo].[Version] WITH CHECK CHECK CONSTRAINT [FK_Version_Catalog_CatalogId];

ALTER TABLE [dbo].[Version] WITH CHECK CHECK CONSTRAINT [FK_Version_Plan_MasterPlanId];

ALTER TABLE [dbo].[VersionRomItem] WITH CHECK CHECK CONSTRAINT [FK_VersionRomItem_RomItem_RomItemId];

ALTER TABLE [dbo].[VersionRomItem] WITH CHECK CHECK CONSTRAINT [FK_VersionRomItem_Version_VersionId];


GO
PRINT N'Actualización completada.';


GO


