CREATE TABLE [dbo].[Catalog] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (100) NULL,
    [Code]    NVARCHAR (100) NULL,
    [Range]   NVARCHAR (100) NULL,
    [Enabled] BIT            NOT NULL,
    [EducationToolOriginId] INT NULL,
    CONSTRAINT [PK_Catalog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Catalog_EducationToolOrigin_EducationToolOriginId] FOREIGN KEY ([EducationToolOriginId]) REFERENCES [EducationToolOrigin]([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Catalog_Code_EducationToolOriginId] ON [dbo].[Catalog]
(
  [Code] ASC,
  [EducationToolOriginId] ASC
) WHERE ([Code] IS NOT NULL);


