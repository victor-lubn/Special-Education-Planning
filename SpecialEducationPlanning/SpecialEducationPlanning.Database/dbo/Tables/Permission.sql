CREATE TABLE [dbo].[Permission] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [DescriptionCode] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([Id] ASC)
);










GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Permission_Name]
    ON [dbo].[Permission]([Name] ASC);

