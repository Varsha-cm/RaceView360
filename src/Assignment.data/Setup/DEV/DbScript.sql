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
    [ActionId] int NOT NULL,
    [PermissionId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_RoleActionPermission] PRIMARY KEY ([OperationId]),
    CONSTRAINT [FK_RoleActionPermission_Actions_ActionId] FOREIGN KEY ([ActionId]) REFERENCES [Actions] ([ActionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleActionPermission_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([PermissionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleActionPermission_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [PasswordHash] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [FullName] nvarchar(max) NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrganizationUsers] (
    [OrgUserId] int NOT NULL IDENTITY,
    [CreatedTimestamp] datetime2 NULL,
    [ModifiedTimestamp] datetime2 NULL,
    [UserId] int NOT NULL,
    [OrganizationId] int NOT NULL,
    CONSTRAINT [PK_OrganizationUsers] PRIMARY KEY ([OrgUserId]),
    CONSTRAINT [FK_OrganizationUsers_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrganizationUsers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ActionId', N'ActionName', N'CreatedTimestamp', N'ModifiedTimestamp') AND [object_id] = OBJECT_ID(N'[Actions]'))
    SET IDENTITY_INSERT [Actions] ON;
INSERT INTO [Actions] ([ActionId], [ActionName], [CreatedTimestamp], [ModifiedTimestamp])
VALUES (1, N'Raiden Settings', '2023-10-10T16:56:16.8023917+05:30', '2023-10-10T16:56:16.8023918+05:30'),
(2, N'Manage Organization', '2023-10-10T16:56:16.8023919+05:30', '2023-10-10T16:56:16.8023919+05:30'),
(3, N'Manage Application', '2023-10-10T16:56:16.8023920+05:30', '2023-10-10T16:56:16.8023921+05:30'),
(4, N'Org Log', '2023-10-10T16:56:16.8023922+05:30', '2023-10-10T16:56:16.8023922+05:30'),
(5, N'App Log', '2023-10-10T16:56:16.8023923+05:30', '2023-10-10T16:56:16.8023923+05:30'),
(6, N'Manage SuperAdmin', '2023-10-10T16:56:16.8023924+05:30', '2023-10-10T16:56:16.8023926+05:30'),
(7, N'Manage OrgAdmin', '2023-10-10T16:56:16.8023927+05:30', '2023-10-10T16:56:16.8023927+05:30'),
(8, N'Manage AppAdmin', '2023-10-10T16:56:16.8023928+05:30', '2023-10-10T16:56:16.8023928+05:30');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ActionId', N'ActionName', N'CreatedTimestamp', N'ModifiedTimestamp') AND [object_id] = OBJECT_ID(N'[Actions]'))
    SET IDENTITY_INSERT [Actions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] ON;
INSERT INTO [Organizations] ([OrganizationId], [CreatedTimestamp], [IsActive], [ModifiedTimestamp], [OrganizationEmail], [OrganizationName], [OrganizationPhone])
VALUES (1, '2023-10-10T16:56:16.8023941+05:30', CAST(1 AS bit), '2023-10-10T16:56:16.8023942+05:30', N'orgA@example.com', N'Organization A', N'1234567890'),
(2, '2023-10-10T16:56:16.8023943+05:30', CAST(1 AS bit), '2023-10-10T16:56:16.8023944+05:30', N'orgB@example.com', N'Organization B', N'9876543210'),
(3, '2023-10-10T16:56:16.8023945+05:30', CAST(1 AS bit), '2023-10-10T16:56:16.8023945+05:30', N'orgC@example.com', N'Organization C', N'5555555555');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([PermissionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionName])
VALUES (1, '2023-10-10T16:56:16.8023838+05:30', '2023-10-10T16:56:16.8023839+05:30', N'View'),
(2, '2023-10-10T16:56:16.8023840+05:30', '2023-10-10T16:56:16.8023840+05:30', N'Create'),
(3, '2023-10-10T16:56:16.8023841+05:30', '2023-10-10T16:56:16.8023842+05:30', N'Edit'),
(4, '2023-10-10T16:56:16.8023842+05:30', '2023-10-10T16:56:16.8023843+05:30', N'Delete'),
(5, '2023-10-10T16:56:16.8023843+05:30', '2023-10-10T16:56:16.8023844+05:30', N'Enable'),
(6, '2023-10-10T16:56:16.8023845+05:30', '2023-10-10T16:56:16.8023845+05:30', N'Disable');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [CreatedTimestamp], [ModifiedTimestamp], [RoleName])
VALUES (1, '2023-10-10T16:56:16.8023726+05:30', '2023-10-10T16:56:16.8023738+05:30', N'SuperAdmin'),
(2, '2023-10-10T16:56:16.8023739+05:30', '2023-10-10T16:56:16.8023740+05:30', N'OrgAdmin'),
(3, '2023-10-10T16:56:16.8023740+05:30', '2023-10-10T16:56:16.8023741+05:30', N'AppAdmin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OperationId', N'ActionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RoleActionPermission]'))
    SET IDENTITY_INSERT [RoleActionPermission] ON;
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (1, 1, '2023-10-10T16:56:16.8023972+05:30', '2023-10-10T16:56:16.8023973+05:30', 1, 1),
(2, 1, '2023-10-10T16:56:16.8023974+05:30', '2023-10-10T16:56:16.8023975+05:30', 2, 1),
(3, 1, '2023-10-10T16:56:16.8023976+05:30', '2023-10-10T16:56:16.8023976+05:30', 3, 1),
(4, 1, '2023-10-10T16:56:16.8023977+05:30', '2023-10-10T16:56:16.8023978+05:30', 4, 1),
(5, 2, '2023-10-10T16:56:16.8023979+05:30', '2023-10-10T16:56:16.8023979+05:30', 1, 1),
(6, 2, '2023-10-10T16:56:16.8023981+05:30', '2023-10-10T16:56:16.8023981+05:30', 2, 1),
(7, 2, '2023-10-10T16:56:16.8023982+05:30', '2023-10-10T16:56:16.8023983+05:30', 3, 1),
(8, 2, '2023-10-10T16:56:16.8023985+05:30', '2023-10-10T16:56:16.8023985+05:30', 4, 1),
(9, 2, '2023-10-10T16:56:16.8023986+05:30', '2023-10-10T16:56:16.8023987+05:30', 5, 1),
(10, 2, '2023-10-10T16:56:16.8023988+05:30', '2023-10-10T16:56:16.8023988+05:30', 6, 1),
(11, 3, '2023-10-10T16:56:16.8023989+05:30', '2023-10-10T16:56:16.8023990+05:30', 1, 1),
(12, 3, '2023-10-10T16:56:16.8023991+05:30', '2023-10-10T16:56:16.8023991+05:30', 2, 1),
(13, 3, '2023-10-10T16:56:16.8023992+05:30', '2023-10-10T16:56:16.8023993+05:30', 3, 1),
(14, 3, '2023-10-10T16:56:16.8023994+05:30', '2023-10-10T16:56:16.8023994+05:30', 4, 1),
(15, 3, '2023-10-10T16:56:16.8023995+05:30', '2023-10-10T16:56:16.8023996+05:30', 5, 1),
(16, 3, '2023-10-10T16:56:16.8023997+05:30', '2023-10-10T16:56:16.8023997+05:30', 6, 1),
(17, 4, '2023-10-10T16:56:16.8023998+05:30', '2023-10-10T16:56:16.8023999+05:30', 1, 1),
(18, 5, '2023-10-10T16:56:16.8024000+05:30', '2023-10-10T16:56:16.8024000+05:30', 1, 1),
(19, 6, '2023-10-10T16:56:16.8024004+05:30', '2023-10-10T16:56:16.8024004+05:30', 1, 1),
(20, 6, '2023-10-10T16:56:16.8024005+05:30', '2023-10-10T16:56:16.8024005+05:30', 2, 1),
(21, 6, '2023-10-10T16:56:16.8024006+05:30', '2023-10-10T16:56:16.8024007+05:30', 3, 1),
(22, 6, '2023-10-10T16:56:16.8024008+05:30', '2023-10-10T16:56:16.8024008+05:30', 4, 1),
(23, 7, '2023-10-10T16:56:16.8024009+05:30', '2023-10-10T16:56:16.8024010+05:30', 1, 1),
(24, 7, '2023-10-10T16:56:16.8024011+05:30', '2023-10-10T16:56:16.8024011+05:30', 2, 1),
(25, 7, '2023-10-10T16:56:16.8024012+05:30', '2023-10-10T16:56:16.8024013+05:30', 3, 1),
(26, 7, '2023-10-10T16:56:16.8024014+05:30', '2023-10-10T16:56:16.8024014+05:30', 4, 1),
(27, 8, '2023-10-10T16:56:16.8024015+05:30', '2023-10-10T16:56:16.8024016+05:30', 1, 1),
(28, 8, '2023-10-10T16:56:16.8024017+05:30', '2023-10-10T16:56:16.8024017+05:30', 2, 1),
(29, 8, '2023-10-10T16:56:16.8024018+05:30', '2023-10-10T16:56:16.8024019+05:30', 3, 1),
(30, 8, '2023-10-10T16:56:16.8024023+05:30', '2023-10-10T16:56:16.8024023+05:30', 4, 1),
(31, 6, '2023-10-10T16:56:16.8024024+05:30', '2023-10-10T16:56:16.8024025+05:30', 1, 1),
(32, 6, '2023-10-10T16:56:16.8024026+05:30', '2023-10-10T16:56:16.8024026+05:30', 2, 1),
(33, 6, '2023-10-10T16:56:16.8024027+05:30', '2023-10-10T16:56:16.8024027+05:30', 3, 1),
(34, 1, '2023-10-10T16:56:16.8024029+05:30', '2023-10-10T16:56:16.8024029+05:30', 1, 2),
(35, 3, '2023-10-10T16:56:16.8024030+05:30', '2023-10-10T16:56:16.8024030+05:30', 1, 2),
(36, 3, '2023-10-10T16:56:16.8024031+05:30', '2023-10-10T16:56:16.8024032+05:30', 2, 2),
(37, 3, '2023-10-10T16:56:16.8024033+05:30', '2023-10-10T16:56:16.8024033+05:30', 3, 2),
(38, 3, '2023-10-10T16:56:16.8024034+05:30', '2023-10-10T16:56:16.8024035+05:30', 4, 2),
(39, 3, '2023-10-10T16:56:16.8024036+05:30', '2023-10-10T16:56:16.8024036+05:30', 5, 2),
(40, 3, '2023-10-10T16:56:16.8024037+05:30', '2023-10-10T16:56:16.8024038+05:30', 6, 2),
(41, 2, '2023-10-10T16:56:16.8024040+05:30', '2023-10-10T16:56:16.8024040+05:30', 3, 2),
(42, 3, '2023-10-10T16:56:16.8024041+05:30', '2023-10-10T16:56:16.8024042+05:30', 6, 2);
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (43, 4, '2023-10-10T16:56:16.8024042+05:30', '2023-10-10T16:56:16.8024043+05:30', 1, 2),
(44, 5, '2023-10-10T16:56:16.8024044+05:30', '2023-10-10T16:56:16.8024044+05:30', 1, 2),
(45, 7, '2023-10-10T16:56:16.8024045+05:30', '2023-10-10T16:56:16.8024046+05:30', 1, 2),
(46, 7, '2023-10-10T16:56:16.8024047+05:30', '2023-10-10T16:56:16.8024047+05:30', 2, 2),
(47, 7, '2023-10-10T16:56:16.8024048+05:30', '2023-10-10T16:56:16.8024049+05:30', 3, 2),
(48, 7, '2023-10-10T16:56:16.8024050+05:30', '2023-10-10T16:56:16.8024050+05:30', 4, 2),
(49, 8, '2023-10-10T16:56:16.8024051+05:30', '2023-10-10T16:56:16.8024052+05:30', 1, 2),
(50, 8, '2023-10-10T16:56:16.8024053+05:30', '2023-10-10T16:56:16.8024053+05:30', 2, 2),
(51, 8, '2023-10-10T16:56:16.8024054+05:30', '2023-10-10T16:56:16.8024055+05:30', 3, 2),
(52, 8, '2023-10-10T16:56:16.8024056+05:30', '2023-10-10T16:56:16.8024056+05:30', 4, 2),
(53, 3, '2023-10-10T16:56:16.8024057+05:30', '2023-10-10T16:56:16.8024058+05:30', 3, 3),
(54, 5, '2023-10-10T16:56:16.8024059+05:30', '2023-10-10T16:56:16.8024059+05:30', 1, 3),
(55, 8, '2023-10-10T16:56:16.8024060+05:30', '2023-10-10T16:56:16.8024061+05:30', 1, 3),
(56, 8, '2023-10-10T16:56:16.8024062+05:30', '2023-10-10T16:56:16.8024062+05:30', 2, 3),
(57, 8, '2023-10-10T16:56:16.8024063+05:30', '2023-10-10T16:56:16.8024064+05:30', 3, 3),
(58, 8, '2023-10-10T16:56:16.8024064+05:30', '2023-10-10T16:56:16.8024065+05:30', 4, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OperationId', N'ActionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RoleActionPermission]'))
    SET IDENTITY_INSERT [RoleActionPermission] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Email', N'FullName', N'PasswordHash', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([UserId], [Email], [FullName], [PasswordHash], [RoleId])
VALUES (1, N'Spurtree@example.com', N'Spurtree', N'Spurtree@1234', 1),
(2, N'Abhay@example.com', N'Abhay', N'Abhay@1234', 2),
(3, N'Bhoomi@example.com', N'Bhoomi', N'Bhoomika@1234', 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Email', N'FullName', N'PasswordHash', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrgUserId', N'CreatedTimestamp', N'ModifiedTimestamp', N'OrganizationId', N'UserId') AND [object_id] = OBJECT_ID(N'[OrganizationUsers]'))
    SET IDENTITY_INSERT [OrganizationUsers] ON;
INSERT INTO [OrganizationUsers] ([OrgUserId], [CreatedTimestamp], [ModifiedTimestamp], [OrganizationId], [UserId])
VALUES (1, '2023-10-10T16:56:16.8023955+05:30', '2023-10-10T16:56:16.8023956+05:30', 1, 1),
(2, '2023-10-10T16:56:16.8023957+05:30', '2023-10-10T16:56:16.8023958+05:30', 1, 2),
(3, '2023-10-10T16:56:16.8023959+05:30', '2023-10-10T16:56:16.8023959+05:30', 3, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrgUserId', N'CreatedTimestamp', N'ModifiedTimestamp', N'OrganizationId', N'UserId') AND [object_id] = OBJECT_ID(N'[OrganizationUsers]'))
    SET IDENTITY_INSERT [OrganizationUsers] OFF;
GO

CREATE INDEX [IX_OrganizationUsers_OrganizationId] ON [OrganizationUsers] ([OrganizationId]);
GO

CREATE INDEX [IX_OrganizationUsers_UserId] ON [OrganizationUsers] ([UserId]);
GO

CREATE INDEX [IX_RoleActionPermission_ActionId] ON [RoleActionPermission] ([ActionId]);
GO

CREATE INDEX [IX_RoleActionPermission_PermissionId] ON [RoleActionPermission] ([PermissionId]);
GO

CREATE INDEX [IX_RoleActionPermission_RoleId] ON [RoleActionPermission] ([RoleId]);
GO

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231010112616_test420', N'7.0.11');
GO

COMMIT;
GO

