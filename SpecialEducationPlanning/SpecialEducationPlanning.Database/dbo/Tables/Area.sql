CREATE TABLE [dbo].[Area] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [RegionId] INT            NOT NULL,
    [KeyName]  NVARCHAR (100) NULL,
    CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Area_Region_RegionId] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([Id]) ON DELETE CASCADE
);








GO
CREATE NONCLUSTERED INDEX [IX_Area_RegionId]
    ON [dbo].[Area]([RegionId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Area_KeyName]
    ON [dbo].[Area]([KeyName] ASC) WHERE ([KeyName] IS NOT NULL);

