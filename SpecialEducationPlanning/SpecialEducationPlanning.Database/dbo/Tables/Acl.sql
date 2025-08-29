CREATE TABLE [dbo].[Acl] (
    [EntityType] NVARCHAR (20) NOT NULL,
    [EntityId]   INT           NOT NULL,
    [UserId]     INT           NOT NULL,
    CONSTRAINT [PK_Acl] PRIMARY KEY CLUSTERED ([UserId] ASC, [EntityType] ASC, [EntityId] ASC)
);








GO
CREATE NONCLUSTERED INDEX [IX_Acl_EntityType_EntityId]
    ON [dbo].[Acl]([EntityType] ASC, [EntityId] ASC);

