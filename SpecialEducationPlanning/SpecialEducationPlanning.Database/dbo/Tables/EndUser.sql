CREATE TABLE [dbo].[EndUser] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Address0]         NVARCHAR (500) NULL,
    [Address1]         NVARCHAR (100) NULL,
    [Address2]         NVARCHAR (100) NULL,
    [Address3]         NVARCHAR (100) NULL,
    [Address4]         NVARCHAR (100) NULL,
    [Address5]         NVARCHAR (100) NULL,
    [Comment]          NVARCHAR (500) NULL,
    [ContactEmail]     NVARCHAR (100) NULL,
    [CountryCode]      INT            NULL,
    [FirstName]        NVARCHAR (100) NULL,
    [LandLineNumber]   NVARCHAR (30) NULL,
    [MobileNumber]     NVARCHAR (30) NULL,
    [Postcode]         NVARCHAR (100) NULL,
    [Surname]          NVARCHAR (100) NULL,
    [TitleId]            INT            NULL,
    [UniqueIdentifier] NVARCHAR (MAX) NULL,
    [CreatedDate]      DATETIME2 (7)  NOT NULL,
    [CreationUser]     NVARCHAR (100) NULL,
    [UpdatedDate]      DATETIME2 (7)  NOT NULL,
    [UpdateUser]       NVARCHAR (100) NULL,   
    [FullName] NVARCHAR(500) NULL, 
    CONSTRAINT [PK_EndUser] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_EndUser_Title_TitleId] FOREIGN KEY ([TitleId]) REFERENCES [dbo].[Title] ([Id])
);








GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EndUser_Surname_Postcode_Address1]
    ON [dbo].[EndUser]([Surname] ASC, [Postcode] ASC, [Address1] ASC) WHERE ([Surname] IS NOT NULL AND [Postcode] IS NOT NULL AND [Address1] IS NOT NULL);

