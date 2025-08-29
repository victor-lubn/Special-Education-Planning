CREATE TABLE [dbo].[Country] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [KeyName] NVARCHAR (100) NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([Id] ASC)
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Country_KeyName]
    ON [dbo].[Country]([KeyName] ASC) WHERE ([KeyName] IS NOT NULL);

