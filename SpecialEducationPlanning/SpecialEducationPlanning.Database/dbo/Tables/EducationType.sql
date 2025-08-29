CREATE TABLE [dbo].[HousingType] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [HousingSpecificationId]    INT            NOT NULL,
    [Code]                      NVARCHAR (100) NOT NULL,
    [Name]                      NVARCHAR (100) NULL,
    CONSTRAINT [PK_HousingType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HousingType_HousingSpecification_HousingSpecificationId] FOREIGN KEY ([HousingSpecificationId]) REFERENCES [dbo].[HousingSpecification] ([Id]) ON DELETE CASCADE,
);

GO