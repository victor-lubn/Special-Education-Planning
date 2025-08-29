CREATE TABLE [dbo].[PermissionRole] (
    [Id]           INT IDENTITY (1, 1) NOT NULL,
    [PermissionId] INT NOT NULL,
    [RoleId]       INT NOT NULL,
    CONSTRAINT [PK_PermissionRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PermissionRole_Permission_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission] ([Id]),
    CONSTRAINT [FK_PermissionRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id])
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_PermissionRole_RoleId]
    ON [dbo].[PermissionRole]([RoleId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PermissionRole_PermissionId_RoleId]
    ON [dbo].[PermissionRole]([PermissionId] ASC, [RoleId] ASC);

