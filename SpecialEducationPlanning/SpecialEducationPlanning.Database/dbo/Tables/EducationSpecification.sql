CREATE TABLE [dbo].[HousingSpecification] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [ProjectId]         INT            NOT NULL,
    [Code]              NVARCHAR (100) NOT NULL,
    [Name]              NVARCHAR (100) NULL,
    [PlanState]         INT            NOT NULL,
    CONSTRAINT [PK_HousingSpecification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HousingSpecification_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([Id]) ON DELETE CASCADE
);

GO