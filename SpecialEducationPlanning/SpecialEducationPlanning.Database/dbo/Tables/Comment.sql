CREATE TABLE [dbo].[Comment] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [EntityName] NVARCHAR (100) NULL,
    [EntityId]   INT            NOT NULL,
    [User]       NVARCHAR (100) NULL,
    [Text]       NVARCHAR (MAX) NULL,
    [CreatedDate] DATETIME2 NOT NULL, 
    [CreationUser] NVARCHAR(100) NULL, 
    [UpdatedDate] DATETIME2 NOT NULL, 
    [UpdateUser] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED ([Id] ASC)
);





