CREATE TABLE [dbo].[Log] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Message]         NVARCHAR (MAX) NULL,
    [MessageTemplate] NVARCHAR (MAX) NULL,
    [Level]           NVARCHAR (100) NULL,
    [TimeStamp]       DATETIME2 (7)  NOT NULL,
    [Exception]       NVARCHAR (MAX) NULL,
    [Properties]      NVARCHAR (MAX) NULL,
    [ExternalSource] BIT NULL, 
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_Log_TimeStamp]
    ON [dbo].[Log]([TimeStamp] ASC);

