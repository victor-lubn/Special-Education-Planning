CREATE TABLE [dbo].[Action] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [ActionType]     INT            NOT NULL,
    [AdditionalInfo] NVARCHAR (MAX) NULL,
    [Date]           DATETIME2 (7)  NOT NULL,
    [EntityId]       INT            NOT NULL,
    [EntityName]     NVARCHAR (100) NULL,
    [User]           NVARCHAR (100) NULL,
    CONSTRAINT [PK_Action] PRIMARY KEY CLUSTERED ([Id] ASC)
);








GO

CREATE NONCLUSTERED INDEX [IX_Action_EntityId_EntityName] ON [dbo].[Action] ([EntityId], [EntityName]) WITH (ONLINE = ON)

GO
CREATE NONCLUSTERED INDEX [IX_Action_Date] ON [dbo].[Action] ([Date]) WITH (ONLINE = ON)