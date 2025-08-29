CREATE TABLE [dbo].[Plan] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Title]               NVARCHAR (20)  NULL,
    [LastOpen]            DATETIME2 (7)  NOT NULL,
    [CatalogId]           INT            NOT NULL,
    [ProjectId]           INT            NOT NULL,
    [KeyName]             NVARCHAR (MAX) NULL,
    [Survey]              BIT            NOT NULL,
    [PlanCode]            NVARCHAR (20)  NULL,
    [EndUserId]           INT            NULL,
    [BuilderId]           INT            NULL,
    [EducationerId]          INT            NULL,
    [PlanState]           INT            NOT NULL,
    [MasterVersionId]     INT            NULL,   
    [IsStarred]           BIT            NOT NULL,
    [IsTemplate]          BIT            NULL,
    [BuilderTradingName]  NVARCHAR (500) NULL,
    [CreatedDate] DATETIME2 NOT NULL, 
    [CreationUser] NVARCHAR(100) NULL, 
    [UpdatedDate] DATETIME2 NOT NULL, 
    [UpdateUser] NVARCHAR(100) NULL, 
    [PlanType] NVARCHAR(500) NULL, 
    [CadFilePlanId] NVARCHAR(20) NULL, 
    [PlanName] NVARCHAR(50) NULL, 
    [OfflineSyncDate] DATETIME2 NULL, 
    [PlanReference] NVARCHAR(50) NULL, 
    [HousingSpecificationId] INT NULL, 
    [HousingTypeId] INT NULL,
    [EducationToolOriginId] INT NULL,
    CONSTRAINT [PK_Plan] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Plan_Builder_BuilderId] FOREIGN KEY ([BuilderId]) REFERENCES [dbo].[Builder] ([Id]),
    CONSTRAINT [FK_Plan_Catalog_CatalogId] FOREIGN KEY ([CatalogId]) REFERENCES [dbo].[Catalog] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Plan_EndUser_EndUserId] FOREIGN KEY ([EndUserId]) REFERENCES [dbo].[EndUser] ([Id]),
    CONSTRAINT [FK_Plan_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Plan_User_EducationerId] FOREIGN KEY ([EducationerId]) REFERENCES [dbo].[User] ([Id]), 
    CONSTRAINT [FK_Plan_Version_MasterVersionId] FOREIGN KEY ([MasterVersionId]) REFERENCES [Version]([Id]),
    CONSTRAINT [FK_Plan_EducationToolOrigin_EducationToolOriginId] FOREIGN KEY ([EducationToolOriginId]) REFERENCES [EducationToolOrigin]([Id])
);














GO
CREATE NONCLUSTERED INDEX [IX_Plan_ProjectId]
    ON [dbo].[Plan]([ProjectId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Plan_EndUserId]
    ON [dbo].[Plan]([EndUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Plan_EducationerId]
    ON [dbo].[Plan]([EducationerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Plan_BuilderId]
    ON [dbo].[Plan]([BuilderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Plan_CatalogId]
    ON [dbo].[Plan]([CatalogId] ASC);


GO

CREATE NONCLUSTERED INDEX [IX_Plan_PlanState_EndUserId] 
ON [dbo].[Plan] ([PlanState], [EndUserId]) INCLUDE ([PlanCode]) WITH (ONLINE = ON)


GO

CREATE NONCLUSTERED INDEX [IX_Plan_UpdatedDate] ON [dbo].[Plan] ([UpdatedDate]) WITH (ONLINE = ON)


