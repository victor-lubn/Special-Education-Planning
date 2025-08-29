CREATE TABLE [dbo].[UserRole] (
    [Id]     INT IDENTITY (1, 1) NOT NULL,
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
);








GO



GO
CREATE NONCLUSTERED INDEX [IX_UserRole_RoleId]
    ON [dbo].[UserRole]([RoleId] ASC);


GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserRole_UserId_RoleId]
    ON [dbo].[UserRole]([UserId] ASC, [RoleId] ASC);

