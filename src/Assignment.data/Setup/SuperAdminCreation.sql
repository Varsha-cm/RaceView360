IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Actions] (
    [ActionId] int NOT NULL IDENTITY,
    [ActionName] nvarchar(100) NOT NULL,
    [CreatedTimestamp] datetime2 NULL,
    [ModifiedTimestamp] datetime2 NULL,
    CONSTRAINT [PK_Actions] PRIMARY KEY ([ActionId])
);
GO

CREATE TABLE [Organizations] (
    [OrganizationId] int NOT NULL IDENTITY,
    [OrganizationName] nvarchar(100) NOT NULL,
    [OrganizationEmail] nvarchar(100) NOT NULL,
    [OrganizationPhone] nvarchar(max) NOT NULL,
    [CreatedTimestamp] datetime2 NULL,
    [ModifiedTimestamp] datetime2 NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Organizations] PRIMARY KEY ([OrganizationId])
);
GO

CREATE TABLE [Permissions] (
    [PermissionId] int NOT NULL IDENTITY,
    [PermissionName] nvarchar(100) NOT NULL,
    [CreatedTimestamp] datetime2 NULL,
    [ModifiedTimestamp] datetime2 NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([PermissionId])
);
GO

CREATE TABLE [Roles] (
    [RoleId] int NOT NULL IDENTITY,
    [RoleName] nvarchar(100) NOT NULL,
    [CreatedTimestamp] datetime2 NULL,
    [ModifiedTimestamp] datetime2 NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleId])
);
GO

CREATE TABLE [RoleActionPermission] (
    [OperationId] int NOT NULL IDENTITY,
    [CreatedTimestamp] datetime2 NULL,
    [ModifiedTimestamp] datetime2 NULL,
    [ActionId1] int NOT NULL,
    [PermissionId1] int NOT NULL,
    [RoleId1] int NOT NULL,
    CONSTRAINT [PK_RoleActionPermission] PRIMARY KEY ([OperationId]),
    CONSTRAINT [FK_RoleActionPermission_Actions_ActionId1] FOREIGN KEY ([ActionId1]) REFERENCES [Actions] ([ActionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleActionPermission_Permissions_PermissionId1] FOREIGN KEY ([PermissionId1]) REFERENCES [Permissions] ([PermissionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleActionPermission_Roles_RoleId1] FOREIGN KEY ([RoleId1]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [PasswordHash] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [FullName] nvarchar(max) NULL,
    [RoleId1] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Users_Roles_RoleId1] FOREIGN KEY ([RoleId1]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrganizationUsers] (
    [OrgUserId] int NOT NULL IDENTITY,
    [CreatedTimestamp] datetime2 NULL,
    [ModifiedTimestamp] datetime2 NULL,
    [UserId1] int NOT NULL,
    [OrganizationId1] int NOT NULL,
    CONSTRAINT [PK_OrganizationUsers] PRIMARY KEY ([OrgUserId]),
    CONSTRAINT [FK_OrganizationUsers_Organizations_OrganizationId1] FOREIGN KEY ([OrganizationId1]) REFERENCES [Organizations] ([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrganizationUsers_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([PermissionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionName])
VALUES (1, '2023-10-06T09:10:51.5146755+05:30', '2023-10-06T09:10:51.5146756+05:30', N'View'),
(2, '2023-10-06T09:10:51.5146759+05:30', '2023-10-06T09:10:51.5146760+05:30', N'Create'),
(3, '2023-10-06T09:10:51.5146761+05:30', '2023-10-06T09:10:51.5146762+05:30', N'Edit'),
(4, '2023-10-06T09:10:51.5146764+05:30', '2023-10-06T09:10:51.5146765+05:30', N'Delete'),
(5, '2023-10-06T09:10:51.5146766+05:30', '2023-10-06T09:10:51.5146767+05:30', N'Enable'),
(6, '2023-10-06T09:10:51.5146769+05:30', '2023-10-06T09:10:51.5146770+05:30', N'Disable');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [CreatedTimestamp], [ModifiedTimestamp], [RoleName])
VALUES (1, '2023-10-06T09:10:51.5146538+05:30', '2023-10-06T09:10:51.5146553+05:30', N'SuperAdmin'),
(2, '2023-10-06T09:10:51.5146556+05:30', '2023-10-06T09:10:51.5146557+05:30', N'OrgAdmin'),
(3, '2023-10-06T09:10:51.5146558+05:30', '2023-10-06T09:10:51.5146559+05:30', N'AppAdmin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

CREATE INDEX [IX_OrganizationUsers_OrganizationId1] ON [OrganizationUsers] ([OrganizationId1]);
GO

CREATE INDEX [IX_OrganizationUsers_UserId1] ON [OrganizationUsers] ([UserId1]);
GO

CREATE INDEX [IX_RoleActionPermission_ActionId1] ON [RoleActionPermission] ([ActionId1]);
GO

CREATE INDEX [IX_RoleActionPermission_PermissionId1] ON [RoleActionPermission] ([PermissionId1]);
GO

CREATE INDEX [IX_RoleActionPermission_RoleId1] ON [RoleActionPermission] ([RoleId1]);
GO

CREATE INDEX [IX_Users_RoleId1] ON [Users] ([RoleId1]);
GO
-- Insert sample values into Users table (including a super user)
INSERT INTO Users (PasswordHash, Email, FullName, RoleId1)
VALUES
    ('Spurtree@1234', 'Spurtree@example.com', 'Super Admin', 1);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231006034052_test', N'7.0.11');
GO

COMMIT;
GO

