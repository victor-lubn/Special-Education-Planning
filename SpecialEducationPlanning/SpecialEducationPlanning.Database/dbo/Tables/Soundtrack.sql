CREATE TABLE [dbo].[Soundtrack] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [CreatedDate] DATETIME2 (7)  NOT NULL,
    [Display]      NVARCHAR (MAX) NULL,
    [Code]         NVARCHAR (450) NULL,   
    CONSTRAINT [PK_Soundtrack] PRIMARY KEY CLUSTERED ([Id] ASC)
);





GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Soundtrack_Code]
    ON [dbo].[Soundtrack]([Code] ASC) WHERE ([Code] IS NOT NULL);

