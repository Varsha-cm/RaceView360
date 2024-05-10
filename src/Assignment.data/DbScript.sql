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
    [OrgCode] nvarchar(12) NOT NULL,
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
    [Password] nvarchar(max) NULL,
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
VALUES (1, N'Raiden Settings', '2023-10-16T13:14:40.4520436+05:30', '2023-10-16T13:14:40.4520436+05:30'),
(2, N'Manage Organization', '2023-10-16T13:14:40.4520437+05:30', '2023-10-16T13:14:40.4520438+05:30'),
(3, N'Manage Application', '2023-10-16T13:14:40.4520439+05:30', '2023-10-16T13:14:40.4520439+05:30'),
(4, N'Org Log', '2023-10-16T13:14:40.4520440+05:30', '2023-10-16T13:14:40.4520440+05:30'),
(5, N'App Log', '2023-10-16T13:14:40.4520441+05:30', '2023-10-16T13:14:40.4520442+05:30'),
(6, N'Manage SuperAdmin', '2023-10-16T13:14:40.4520442+05:30', '2023-10-16T13:14:40.4520443+05:30'),
(7, N'Manage OrgAdmin', '2023-10-16T13:14:40.4520443+05:30', '2023-10-16T13:14:40.4520444+05:30'),
(8, N'Manage AppAdmin', '2023-10-16T13:14:40.4520444+05:30', '2023-10-16T13:14:40.4520445+05:30');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ActionId', N'ActionName', N'CreatedTimestamp', N'ModifiedTimestamp') AND [object_id] = OBJECT_ID(N'[Actions]'))
    SET IDENTITY_INSERT [Actions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrgCode', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] ON;
INSERT INTO [Organizations] ([OrganizationId], [CreatedTimestamp], [IsActive], [ModifiedTimestamp], [OrgCode], [OrganizationEmail], [OrganizationName], [OrganizationPhone])
VALUES (1, '2023-10-16T13:14:40.4520459+05:30', CAST(1 AS bit), '2023-10-16T13:14:40.4520460+05:30', N'CD01', N'orgA@example.com', N'Organization A', N'1234567890'),
(2, '2023-10-16T13:14:40.4520462+05:30', CAST(1 AS bit), '2023-10-16T13:14:40.4520462+05:30', N'AB01', N'orgB@example.com', N'Organization B', N'9876543210'),
(3, '2023-10-16T13:14:40.4520464+05:30', CAST(1 AS bit), '2023-10-16T13:14:40.4520464+05:30', N'DC02', N'orgC@example.com', N'Organization C', N'5555555555');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrgCode', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([PermissionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionName])
VALUES (1, '2023-10-16T13:14:40.4520385+05:30', '2023-10-16T13:14:40.4520386+05:30', N'View'),
(2, '2023-10-16T13:14:40.4520387+05:30', '2023-10-16T13:14:40.4520388+05:30', N'Create'),
(3, '2023-10-16T13:14:40.4520388+05:30', '2023-10-16T13:14:40.4520389+05:30', N'Edit'),
(4, '2023-10-16T13:14:40.4520389+05:30', '2023-10-16T13:14:40.4520390+05:30', N'Delete'),
(5, '2023-10-16T13:14:40.4520390+05:30', '2023-10-16T13:14:40.4520391+05:30', N'Enable'),
(6, '2023-10-16T13:14:40.4520392+05:30', '2023-10-16T13:14:40.4520393+05:30', N'Disable');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [CreatedTimestamp], [ModifiedTimestamp], [RoleName])
VALUES (1, '2023-10-16T13:14:40.4520291+05:30', '2023-10-16T13:14:40.4520303+05:30', N'SuperAdmin'),
(2, '2023-10-16T13:14:40.4520304+05:30', '2023-10-16T13:14:40.4520305+05:30', N'OrgAdmin'),
(3, '2023-10-16T13:14:40.4520306+05:30', '2023-10-16T13:14:40.4520307+05:30', N'AppAdmin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OperationId', N'ActionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RoleActionPermission]'))
    SET IDENTITY_INSERT [RoleActionPermission] ON;
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (1, 1, '2023-10-16T13:14:40.4520493+05:30', '2023-10-16T13:14:40.4520494+05:30', 1, 1),
(2, 1, '2023-10-16T13:14:40.4520495+05:30', '2023-10-16T13:14:40.4520496+05:30', 2, 1),
(3, 1, '2023-10-16T13:14:40.4520497+05:30', '2023-10-16T13:14:40.4520497+05:30', 3, 1),
(4, 1, '2023-10-16T13:14:40.4520498+05:30', '2023-10-16T13:14:40.4520499+05:30', 4, 1),
(5, 2, '2023-10-16T13:14:40.4520500+05:30', '2023-10-16T13:14:40.4520500+05:30', 1, 1),
(6, 2, '2023-10-16T13:14:40.4520502+05:30', '2023-10-16T13:14:40.4520502+05:30', 2, 1),
(7, 2, '2023-10-16T13:14:40.4520503+05:30', '2023-10-16T13:14:40.4520504+05:30', 3, 1),
(8, 2, '2023-10-16T13:14:40.4520505+05:30', '2023-10-16T13:14:40.4520506+05:30', 4, 1),
(9, 2, '2023-10-16T13:14:40.4520507+05:30', '2023-10-16T13:14:40.4520507+05:30', 5, 1),
(10, 2, '2023-10-16T13:14:40.4520508+05:30', '2023-10-16T13:14:40.4520509+05:30', 6, 1),
(11, 3, '2023-10-16T13:14:40.4520510+05:30', '2023-10-16T13:14:40.4520510+05:30', 1, 1),
(12, 3, '2023-10-16T13:14:40.4520511+05:30', '2023-10-16T13:14:40.4520512+05:30', 2, 1),
(13, 3, '2023-10-16T13:14:40.4520513+05:30', '2023-10-16T13:14:40.4520513+05:30', 3, 1),
(14, 3, '2023-10-16T13:14:40.4520515+05:30', '2023-10-16T13:14:40.4520515+05:30', 4, 1),
(15, 3, '2023-10-16T13:14:40.4520516+05:30', '2023-10-16T13:14:40.4520517+05:30', 5, 1),
(16, 3, '2023-10-16T13:14:40.4520518+05:30', '2023-10-16T13:14:40.4520518+05:30', 6, 1),
(17, 4, '2023-10-16T13:14:40.4520519+05:30', '2023-10-16T13:14:40.4520520+05:30', 1, 1),
(18, 5, '2023-10-16T13:14:40.4520521+05:30', '2023-10-16T13:14:40.4520522+05:30', 1, 1),
(19, 6, '2023-10-16T13:14:40.4520523+05:30', '2023-10-16T13:14:40.4520523+05:30', 1, 1),
(20, 6, '2023-10-16T13:14:40.4520524+05:30', '2023-10-16T13:14:40.4520525+05:30', 2, 1),
(21, 6, '2023-10-16T13:14:40.4520527+05:30', '2023-10-16T13:14:40.4520527+05:30', 3, 1),
(22, 6, '2023-10-16T13:14:40.4520528+05:30', '2023-10-16T13:14:40.4520528+05:30', 4, 1),
(23, 7, '2023-10-16T13:14:40.4520529+05:30', '2023-10-16T13:14:40.4520530+05:30', 1, 1),
(24, 7, '2023-10-16T13:14:40.4520531+05:30', '2023-10-16T13:14:40.4520532+05:30', 2, 1),
(25, 7, '2023-10-16T13:14:40.4520533+05:30', '2023-10-16T13:14:40.4520533+05:30', 3, 1),
(26, 7, '2023-10-16T13:14:40.4520534+05:30', '2023-10-16T13:14:40.4520534+05:30', 4, 1),
(27, 8, '2023-10-16T13:14:40.4520535+05:30', '2023-10-16T13:14:40.4520536+05:30', 1, 1),
(28, 8, '2023-10-16T13:14:40.4520537+05:30', '2023-10-16T13:14:40.4520537+05:30', 2, 1),
(29, 8, '2023-10-16T13:14:40.4520538+05:30', '2023-10-16T13:14:40.4520539+05:30', 3, 1),
(30, 8, '2023-10-16T13:14:40.4520540+05:30', '2023-10-16T13:14:40.4520541+05:30', 4, 1),
(31, 6, '2023-10-16T13:14:40.4520542+05:30', '2023-10-16T13:14:40.4520542+05:30', 1, 1),
(32, 6, '2023-10-16T13:14:40.4520543+05:30', '2023-10-16T13:14:40.4520544+05:30', 2, 1),
(33, 6, '2023-10-16T13:14:40.4520545+05:30', '2023-10-16T13:14:40.4520546+05:30', 3, 1),
(34, 1, '2023-10-16T13:14:40.4520547+05:30', '2023-10-16T13:14:40.4520547+05:30', 1, 2),
(35, 3, '2023-10-16T13:14:40.4520549+05:30', '2023-10-16T13:14:40.4520550+05:30', 1, 2),
(36, 3, '2023-10-16T13:14:40.4520551+05:30', '2023-10-16T13:14:40.4520551+05:30', 2, 2),
(37, 3, '2023-10-16T13:14:40.4520552+05:30', '2023-10-16T13:14:40.4520553+05:30', 3, 2),
(38, 3, '2023-10-16T13:14:40.4520553+05:30', '2023-10-16T13:14:40.4520554+05:30', 4, 2),
(39, 3, '2023-10-16T13:14:40.4520556+05:30', '2023-10-16T13:14:40.4520557+05:30', 5, 2),
(40, 3, '2023-10-16T13:14:40.4520558+05:30', '2023-10-16T13:14:40.4520559+05:30', 6, 2),
(41, 2, '2023-10-16T13:14:40.4520561+05:30', '2023-10-16T13:14:40.4520562+05:30', 3, 2),
(42, 3, '2023-10-16T13:14:40.4520563+05:30', '2023-10-16T13:14:40.4520563+05:30', 6, 2);
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (43, 4, '2023-10-16T13:14:40.4520565+05:30', '2023-10-16T13:14:40.4520565+05:30', 1, 2),
(44, 5, '2023-10-16T13:14:40.4520567+05:30', '2023-10-16T13:14:40.4520567+05:30', 1, 2),
(45, 7, '2023-10-16T13:14:40.4520568+05:30', '2023-10-16T13:14:40.4520569+05:30', 1, 2),
(46, 7, '2023-10-16T13:14:40.4520570+05:30', '2023-10-16T13:14:40.4520571+05:30', 2, 2),
(47, 7, '2023-10-16T13:14:40.4520573+05:30', '2023-10-16T13:14:40.4520573+05:30', 3, 2),
(48, 7, '2023-10-16T13:14:40.4520574+05:30', '2023-10-16T13:14:40.4520575+05:30', 4, 2),
(49, 8, '2023-10-16T13:14:40.4520576+05:30', '2023-10-16T13:14:40.4520577+05:30', 1, 2),
(50, 8, '2023-10-16T13:14:40.4520578+05:30', '2023-10-16T13:14:40.4520579+05:30', 2, 2),
(51, 8, '2023-10-16T13:14:40.4520581+05:30', '2023-10-16T13:14:40.4520582+05:30', 3, 2),
(52, 8, '2023-10-16T13:14:40.4520584+05:30', '2023-10-16T13:14:40.4520585+05:30', 4, 2),
(53, 3, '2023-10-16T13:14:40.4520586+05:30', '2023-10-16T13:14:40.4520586+05:30', 3, 3),
(54, 5, '2023-10-16T13:14:40.4520589+05:30', '2023-10-16T13:14:40.4520590+05:30', 1, 3),
(55, 8, '2023-10-16T13:14:40.4520591+05:30', '2023-10-16T13:14:40.4520591+05:30', 1, 3),
(56, 8, '2023-10-16T13:14:40.4520592+05:30', '2023-10-16T13:14:40.4520593+05:30', 2, 3),
(57, 8, '2023-10-16T13:14:40.4520594+05:30', '2023-10-16T13:14:40.4520594+05:30', 3, 3),
(58, 8, '2023-10-16T13:14:40.4520595+05:30', '2023-10-16T13:14:40.4520596+05:30', 4, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OperationId', N'ActionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RoleActionPermission]'))
    SET IDENTITY_INSERT [RoleActionPermission] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Email', N'FullName', N'Password', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([UserId], [Email], [FullName], [Password], [RoleId])
VALUES (1, N'Spurtree@example.com', N'Spurtree', N'Spurtree@1234', 1),
(2, N'Abhay@example.com', N'Abhay', N'Abhay@1234', 2),
(3, N'Bhoomi@example.com', N'Bhoomi', N'Bhoomika@1234', 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Email', N'FullName', N'Password', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrgUserId', N'CreatedTimestamp', N'ModifiedTimestamp', N'OrganizationId', N'UserId') AND [object_id] = OBJECT_ID(N'[OrganizationUsers]'))
    SET IDENTITY_INSERT [OrganizationUsers] ON;
INSERT INTO [OrganizationUsers] ([OrgUserId], [CreatedTimestamp], [ModifiedTimestamp], [OrganizationId], [UserId])
VALUES (1, '2023-10-16T13:14:40.4520476+05:30', '2023-10-16T13:14:40.4520477+05:30', 1, 1),
(2, '2023-10-16T13:14:40.4520478+05:30', '2023-10-16T13:14:40.4520479+05:30', 1, 2),
(3, '2023-10-16T13:14:40.4520480+05:30', '2023-10-16T13:14:40.4520480+05:30', 3, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrgUserId', N'CreatedTimestamp', N'ModifiedTimestamp', N'OrganizationId', N'UserId') AND [object_id] = OBJECT_ID(N'[OrganizationUsers]'))
    SET IDENTITY_INSERT [OrganizationUsers] OFF;
GO

CREATE UNIQUE INDEX [IX_Organizations_OrgCode] ON [Organizations] ([OrgCode]);
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
VALUES (N'20231016074440_t420', N'7.0.11');
GO

COMMIT;
GO

