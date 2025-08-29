CREATE TABLE [dbo].[ReleaseInfo] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Title]        NVARCHAR (100) NULL,
    [Subtitle]     NVARCHAR (100) NULL,
	[Version]      NVARCHAR (20) NULL,
	[FusionVersion] NVARCHAR (20) NULL,
    [DateTime]     DATETIME2 (7)  NOT NULL,
    [Document]     NVARCHAR (100) NULL,
    [DocumentPath] NVARCHAR (100) NULL,
    CONSTRAINT [PK_ReleaseInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ReleaseInfo_Version_FusionVersion]
    ON [dbo].[ReleaseInfo]([Version] ASC, [FusionVersion] ASC)