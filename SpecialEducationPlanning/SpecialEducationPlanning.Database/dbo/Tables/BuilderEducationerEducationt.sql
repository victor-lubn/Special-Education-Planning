CREATE TABLE [dbo].[BuilderEducationerAiep] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [BuilderId]  INT NOT NULL,   
    [AiepId]    INT NOT NULL,
    CONSTRAINT [PK_BuilderEducationerAiep] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BuilderEducationerAiep_Builder_BuilderId] FOREIGN KEY ([BuilderId]) REFERENCES [dbo].[Builder] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BuilderEducationerAiep_Aiep_AiepId] FOREIGN KEY ([AiepId]) REFERENCES [dbo].[Aiep] ([Id])   
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BuilderEducationerAiep_BuilderId_AiepId]
    ON [dbo].[BuilderEducationerAiep]([BuilderId] ASC, [AiepId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_BuilderEducationerAiep_AiepId]
    ON [dbo].[BuilderEducationerAiep]([AiepId] ASC);



