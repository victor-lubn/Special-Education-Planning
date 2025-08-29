CREATE TABLE [dbo].[Aiep] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [ReleaseInfoId] INT            NULL,
    [AiepCode]     NVARCHAR (20)  NULL,
    [AreaId]        INT            NOT NULL,
    [Name]          NVARCHAR (100) NULL,
    [Email]         NVARCHAR (100) NULL,
    [Address1]      NVARCHAR (100) NULL,
    [Address2]      NVARCHAR (100) NULL,
    [Address3]      NVARCHAR (100) NULL,
    [Address4]      NVARCHAR (100) NULL,
    [Address5]      NVARCHAR (100) NULL,
    [Address6]      NVARCHAR (100) NULL,
    [Postcode]      NVARCHAR (20)  NULL,
    [PhoneNumber]   NVARCHAR (30)  NULL,
    [FaxNumber]     NVARCHAR (30)  NULL,
    [IpAddress]     NVARCHAR (20)  NULL,
    [MediaServer]   NVARCHAR (20)  NULL,
    [HtmlEmail]     BIT			   NOT NULL DEFAULT ((0))  ,
    [DownloadSpeed] INT            NOT NULL,
    [DownloadLimit] INT            NOT NULL,
    [ManagerId]     INT            NULL,   
    [CreatedDate] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
    [CreationUser] NVARCHAR(100) NULL, 
    [UpdatedDate] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
    [UpdateUser] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_Aiep] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Aiep_Area_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Area] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Aiep_ReleaseInfo_ReleaseInfoId] FOREIGN KEY ([ReleaseInfoId]) REFERENCES [dbo].[ReleaseInfo] ([Id]),
    CONSTRAINT [FK_Aiep_User_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[User] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_Aiep_ReleaseInfoId]
    ON [dbo].[Aiep]([ReleaseInfoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Aiep_ManagerId]
    ON [dbo].[Aiep]([ManagerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Aiep_AreaId]
    ON [dbo].[Aiep]([AreaId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Aiep_AiepCode]
    ON [dbo].[Aiep]([AiepCode] ASC) WHERE ([AiepCode] IS NOT NULL);


