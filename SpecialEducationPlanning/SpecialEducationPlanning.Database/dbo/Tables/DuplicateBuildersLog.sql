CREATE TABLE [dbo].[DuplicateBuildersLog] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [OldBuilderId] VARCHAR (300) NULL,
    [NewBuilderId] INT           NULL,
    [Result]       VARCHAR (200) NULL,
    [CreatedDate]  DATETIME      NULL,
    [ErrorMessage] VARCHAR (MAX) NULL,
    [ErrorState]   VARCHAR (10)  NULL
);

