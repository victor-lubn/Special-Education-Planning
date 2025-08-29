CREATE TABLE [dbo].[UserReleaseInfo]
(
	[Id]         INT IDENTITY (1, 1) NOT NULL,
    [UserId]  INT NOT NULL,   
    [ReleaseInfoId]    INT NOT NULL,
    CONSTRAINT [PK_UserReleaseInfo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserReleaseInfo_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_UserReleaseInfo_ReleaseInfo_ReleaseInfoId] FOREIGN KEY ([ReleaseInfoId]) REFERENCES [dbo].[ReleaseInfo] ([Id])  
)
