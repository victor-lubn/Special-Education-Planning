CREATE TABLE [dbo].[Version] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [KeyName]       NVARCHAR (100) NULL,
    [PlanId]        INT            NOT NULL,
    [Preview]       NVARCHAR (MAX) NULL,
    [Rom]           NVARCHAR (MAX) NULL,
    [PreviewPath]   NVARCHAR (MAX) NULL,
    [RomPath]       NVARCHAR (MAX) NULL,
    [VersionNumber] INT            NOT NULL,
    [VersionNotes]  NVARCHAR (500) NULL,
    [Range]         NVARCHAR (100) NULL,
    [CatalogId]     INT            NOT NULL,
    [ExternalCode]  NVARCHAR (20)  NULL,
    [AiepCode]     NVARCHAR (20)  NULL,   
    [CreatedDate] DATETIME2 NOT NULL, 
    [CreationUser] NVARCHAR(100) NULL, 
    [UpdatedDate] DATETIME2 NOT NULL, 
    [UpdateUser] NVARCHAR(100) NULL, 
    [QuoteOrderNumber] NVARCHAR(20) NULL,
    [EducationTool3DCVersionId] INT NULL,
    [EducationTool3DCPlanId] NVARCHAR(50) NULL,
    [LastKnown3DCVersion] INT NULL,
    [LastKnownCatalogId] INT NULL,
    [LastKnownPreviewPath] NVARCHAR (MAX) NULL,
    [LastKnownRomPath] NVARCHAR (MAX) NULL,
    [FittersPackPath] NVARCHAR(MAX) NULL,
    [FittersPackStatusId] INT NULL,
    [FittersPack3DCJobId] NVARCHAR(50) NULL,
    [FittersPack3DCRequestTime] DATETIME2 NULL,
    [FittersPack3DCEstimatedTime] DATETIME2 NULL,
    CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Version_Catalog_CatalogId] FOREIGN KEY ([CatalogId]) REFERENCES [dbo].[Catalog] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Version_Plan_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plan] ([Id]),
    CONSTRAINT [FK_Version_FittersPackStatus_FittersPackStatusId] FOREIGN KEY ([FittersPackStatusId]) REFERENCES [FittersPackStatus]([Id])
);










GO



GO
CREATE NONCLUSTERED INDEX [IX_Version_PlanId]
    ON [dbo].[Version]([PlanId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version_CatalogId]
    ON [dbo].[Version]([CatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version_AiepCode] 
ON [dbo].[Version] ([AiepCode]) INCLUDE ([PlanId]) WITH (ONLINE = ON)


GO


