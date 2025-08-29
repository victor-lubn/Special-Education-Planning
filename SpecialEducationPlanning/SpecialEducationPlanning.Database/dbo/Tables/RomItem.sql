CREATE TABLE [dbo].[RomItem] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [ItemName]     NVARCHAR (100) NOT NULL,
    [SerialNumber] NVARCHAR (100) NULL,
    [Sku]          NVARCHAR (100) NULL,
    [Range]        NVARCHAR (100) NULL,
    [Colour]       NVARCHAR (100) NULL,
    [Qty]          INT            NOT NULL,
    [Description]  NVARCHAR (128) NULL,
    [Handing]        NVARCHAR (128) NULL,
    [PosNumber]      NVARCHAR (128) NULL,
    [Annotation]     NVARCHAR (128) NULL,
    [OrderCode]      NVARCHAR (128) NULL,
    [CatalogId]    INT            NOT NULL,
    [VersionId]    INT            NOT NULL,
    CONSTRAINT [PK_RomItem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RomItem_Catalog_CatalogId] FOREIGN KEY ([CatalogId]) REFERENCES [dbo].[Catalog] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RomItem_Version_VersionId] FOREIGN KEY ([VersionId]) REFERENCES [dbo].[Version] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_RomItem_CatalogId]
    ON [dbo].[RomItem]([CatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RomItem_VersionId]
    ON [dbo].[RomItem]([VersionId] ASC)
	INCLUDE ([CatalogId],[Colour],[ItemName],[Qty],[Range],[SerialNumber],[Sku]); 



