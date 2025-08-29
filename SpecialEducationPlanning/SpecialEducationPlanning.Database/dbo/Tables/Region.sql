CREATE TABLE [dbo].[Region] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [CountryId] INT            NOT NULL,
    [KeyName]   NVARCHAR (100) NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Region_Country_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_Region_CountryId]
    ON [dbo].[Region]([CountryId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Region_KeyName]
    ON [dbo].[Region]([KeyName] ASC) WHERE ([KeyName] IS NOT NULL);

