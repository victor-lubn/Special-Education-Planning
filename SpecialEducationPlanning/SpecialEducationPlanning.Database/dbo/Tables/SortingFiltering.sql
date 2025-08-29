CREATE TABLE [dbo].[SortingFiltering]
(
	[Id]		INT IDENTITY (1, 1) NOT NULL, 
    [EntityType]	VARCHAR(20) NOT NULL, 
    [PropertyName]		VARCHAR(100) NOT NULL,
	[PropertyText] VARCHAR(100) NULL, 
    CONSTRAINT [PK_Sorting] PRIMARY KEY CLUSTERED ([Id] ASC)
)
