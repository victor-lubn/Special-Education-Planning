CREATE TABLE [dbo].[Project] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [CodeProject]       NVARCHAR (100) NULL,
    [AiepId]           INT            NOT NULL,
    [KeyName]           NVARCHAR (100) NULL,
    [SinglePlanProject] BIT            NOT NULL,
    [CreatedDate]       DATETIME2 (7)  NOT NULL,
    [CreationUser]      NVARCHAR (MAX) NULL,
    [UpdatedDate]       DATETIME2 (7)  NOT NULL,
    [UpdateUser]        NVARCHAR (MAX) NULL,
    [BuilderId]         INT            NULL,
    [IsArchived]        BIT            NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Project_Aiep_AiepId] FOREIGN KEY ([AiepId]) REFERENCES [dbo].[Aiep] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Project_Builder_BuilderId] FOREIGN KEY ([BuilderId]) REFERENCES [dbo].[Builder] ([Id]) ON DELETE SET NULL
);






GO
CREATE NONCLUSTERED INDEX [IX_Project_AiepId]
    ON [dbo].[Project]([AiepId] ASC);


