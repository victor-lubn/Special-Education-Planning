CREATE TABLE [dbo].[User] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CurrentAiepId]   INT            NULL,
    [FirstName]        NVARCHAR (MAX) NULL,
    [FullAclAccess]    BIT            NOT NULL,
    [Surname]          NVARCHAR (MAX) NULL,
    [UniqueIdentifier] NVARCHAR (450) NULL,
    [CreatedDate]      DATETIME2 (7)  NOT NULL,
    [CreationUser]     NVARCHAR (MAX) NULL,
    [Discriminator]    NVARCHAR (MAX) NULL,
    [UpdatedDate]      DATETIME2 (7)  NOT NULL,
    [UpdateUser]       NVARCHAR (MAX) NULL,
    [AiepId]          INT            NULL,
    [ReleaseInfoId]    INT            NULL,
    [Leaver]           BIT            CONSTRAINT [DF_User_Leaver] DEFAULT ((0)) NOT NULL,
    [DelegateToUserId] INT            NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_User_DelegateToUserId_NotNull] CHECK ([Leaver]=(0) OR [DelegateToUserId] IS NOT NULL),   
    CONSTRAINT [FK_User_Aiep_CurrentAiepId] FOREIGN KEY ([CurrentAiepId]) REFERENCES [dbo].[Aiep] ([Id]),
    CONSTRAINT [FK_User_Aiep_AiepId] FOREIGN KEY ([AiepId]) REFERENCES [dbo].[Aiep] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_User_ReleaseInfo_ReleaseInfoId] FOREIGN KEY ([ReleaseInfoId]) REFERENCES [dbo].[ReleaseInfo] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_User_ReleaseInfoId]
    ON [dbo].[User]([ReleaseInfoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_User_AiepId]
    ON [dbo].[User]([AiepId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_User_UniqueIdentifier]
    ON [dbo].[User]([UniqueIdentifier] ASC) WHERE ([UniqueIdentifier] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [IX_User_CurrentAiepId]
    ON [dbo].[User]([CurrentAiepId] ASC);


