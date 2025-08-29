CREATE TABLE [dbo].[EndUserAiep] (
    [Id]        INT IDENTITY (1, 1) NOT NULL,
    [EndUserId] INT NOT NULL,
    [AiepId]   INT NOT NULL,
    CONSTRAINT [PK_EndUserAiep] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EndUserAiep_Aiep_AiepId] FOREIGN KEY ([AiepId]) REFERENCES [dbo].[Aiep] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EndUserAiep_EndUser_EndUserId] FOREIGN KEY ([EndUserId]) REFERENCES [dbo].[EndUser] ([Id]) ON DELETE CASCADE
);





GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EndUserAiep_AiepId_EndUserId]
    ON [dbo].[EndUserAiep]([AiepId] ASC, [EndUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EndUserAiep_EndUserId]
    ON [dbo].[EndUserAiep]([EndUserId] ASC);


