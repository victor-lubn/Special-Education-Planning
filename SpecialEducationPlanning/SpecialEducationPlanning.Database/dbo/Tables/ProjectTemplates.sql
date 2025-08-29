CREATE TABLE [dbo].[ProjectTemplates] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [ProjectId]     INT            NOT NULL,
    [PlanId]        INT            NOT NULL,
    CONSTRAINT [PK_ProjectTemplates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProjectTemplates_Project_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProjectTemplates_Plan_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plan] ([Id]) ON DELETE NO ACTION
);

GO