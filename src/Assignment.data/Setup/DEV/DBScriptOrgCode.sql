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

CREATE TABLE [Applications] (
    [ApplicationId] int NOT NULL IDENTITY,
    [ApplicationName] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [FullName] nvarchar(100) NOT NULL,
    [ApplicationPhone] nvarchar(max) NOT NULL,
    [OrganizationId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [ClientId] int NOT NULL,
    [ClientSecret] nvarchar(max) NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY ([ApplicationId]),
    CONSTRAINT [FK_Applications_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([OrganizationId]) ON DELETE CASCADE
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
    [RefreshToken] nvarchar(max) NULL,
    [TokenExpiryTime] datetime2 NOT NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
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
    [Orgcode] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_OrganizationUsers] PRIMARY KEY ([OrgUserId]),
    CONSTRAINT [FK_OrganizationUsers_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([OrganizationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrganizationUsers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ActionId', N'ActionName', N'CreatedTimestamp', N'ModifiedTimestamp') AND [object_id] = OBJECT_ID(N'[Actions]'))
    SET IDENTITY_INSERT [Actions] ON;
INSERT INTO [Actions] ([ActionId], [ActionName], [CreatedTimestamp], [ModifiedTimestamp])
VALUES (1, N'raiden-settings', '2023-10-19T10:59:17.1399185+05:30', '2023-10-19T10:59:17.1399186+05:30'),
(2, N'manage-organization', '2023-10-19T10:59:17.1399187+05:30', '2023-10-19T10:59:17.1399187+05:30'),
(3, N'manage-application', '2023-10-19T10:59:17.1399188+05:30', '2023-10-19T10:59:17.1399188+05:30'),
(4, N'org-log', '2023-10-19T10:59:17.1399189+05:30', '2023-10-19T10:59:17.1399190+05:30'),
(5, N'app-log', '2023-10-19T10:59:17.1399190+05:30', '2023-10-19T10:59:17.1399191+05:30'),
(6, N'manage-superadmin', '2023-10-19T10:59:17.1399192+05:30', '2023-10-19T10:59:17.1399192+05:30'),
(7, N'manage-orgadmin', '2023-10-19T10:59:17.1399193+05:30', '2023-10-19T10:59:17.1399193+05:30'),
(8, N'manage-appadmin', '2023-10-19T10:59:17.1399194+05:30', '2023-10-19T10:59:17.1399195+05:30');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ActionId', N'ActionName', N'CreatedTimestamp', N'ModifiedTimestamp') AND [object_id] = OBJECT_ID(N'[Actions]'))
    SET IDENTITY_INSERT [Actions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrgCode', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] ON;
INSERT INTO [Organizations] ([OrganizationId], [CreatedTimestamp], [IsActive], [ModifiedTimestamp], [OrgCode], [OrganizationEmail], [OrganizationName], [OrganizationPhone])
VALUES (1, '2023-10-19T10:59:17.1399206+05:30', CAST(1 AS bit), '2023-10-19T10:59:17.1399207+05:30', N'CD01', N'orgA@example.com', N'Organization A', N'1234567890'),
(2, '2023-10-19T10:59:17.1399208+05:30', CAST(1 AS bit), '2023-10-19T10:59:17.1399209+05:30', N'AB01', N'orgB@example.com', N'Organization B', N'9876543210'),
(3, '2023-10-19T10:59:17.1399210+05:30', CAST(1 AS bit), '2023-10-19T10:59:17.1399210+05:30', N'DC02', N'orgC@example.com', N'Organization C', N'5555555555');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrgCode', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([PermissionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionName])
VALUES (1, '2023-10-19T10:59:17.1399144+05:30', '2023-10-19T10:59:17.1399145+05:30', N'view'),
(2, '2023-10-19T10:59:17.1399146+05:30', '2023-10-19T10:59:17.1399147+05:30', N'create'),
(3, '2023-10-19T10:59:17.1399147+05:30', '2023-10-19T10:59:17.1399148+05:30', N'edit'),
(4, '2023-10-19T10:59:17.1399149+05:30', '2023-10-19T10:59:17.1399149+05:30', N'delete'),
(5, '2023-10-19T10:59:17.1399150+05:30', '2023-10-19T10:59:17.1399150+05:30', N'enable'),
(6, '2023-10-19T10:59:17.1399151+05:30', '2023-10-19T10:59:17.1399152+05:30', N'disable');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [CreatedTimestamp], [ModifiedTimestamp], [RoleName])
VALUES (1, '2023-10-19T10:59:17.1399041+05:30', '2023-10-19T10:59:17.1399053+05:30', N'SuperAdmin'),
(2, '2023-10-19T10:59:17.1399054+05:30', '2023-10-19T10:59:17.1399055+05:30', N'OrgAdmin'),
(3, '2023-10-19T10:59:17.1399056+05:30', '2023-10-19T10:59:17.1399056+05:30', N'AppAdmin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OperationId', N'ActionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RoleActionPermission]'))
    SET IDENTITY_INSERT [RoleActionPermission] ON;
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (1, 1, '2023-10-19T10:59:17.1399240+05:30', '2023-10-19T10:59:17.1399240+05:30', 1, 1),
(2, 1, '2023-10-19T10:59:17.1399242+05:30', '2023-10-19T10:59:17.1399242+05:30', 2, 1),
(3, 1, '2023-10-19T10:59:17.1399244+05:30', '2023-10-19T10:59:17.1399244+05:30', 3, 1),
(4, 1, '2023-10-19T10:59:17.1399245+05:30', '2023-10-19T10:59:17.1399246+05:30', 4, 1),
(5, 2, '2023-10-19T10:59:17.1399247+05:30', '2023-10-19T10:59:17.1399247+05:30', 1, 1),
(6, 2, '2023-10-19T10:59:17.1399248+05:30', '2023-10-19T10:59:17.1399249+05:30', 2, 1),
(7, 2, '2023-10-19T10:59:17.1399250+05:30', '2023-10-19T10:59:17.1399250+05:30', 3, 1),
(8, 2, '2023-10-19T10:59:17.1399251+05:30', '2023-10-19T10:59:17.1399252+05:30', 4, 1),
(9, 2, '2023-10-19T10:59:17.1399256+05:30', '2023-10-19T10:59:17.1399257+05:30', 5, 1),
(10, 2, '2023-10-19T10:59:17.1399258+05:30', '2023-10-19T10:59:17.1399258+05:30', 6, 1),
(11, 3, '2023-10-19T10:59:17.1399259+05:30', '2023-10-19T10:59:17.1399260+05:30', 1, 1),
(12, 3, '2023-10-19T10:59:17.1399261+05:30', '2023-10-19T10:59:17.1399261+05:30', 2, 1),
(13, 3, '2023-10-19T10:59:17.1399262+05:30', '2023-10-19T10:59:17.1399263+05:30', 3, 1),
(14, 3, '2023-10-19T10:59:17.1399264+05:30', '2023-10-19T10:59:17.1399264+05:30', 4, 1),
(15, 3, '2023-10-19T10:59:17.1399266+05:30', '2023-10-19T10:59:17.1399266+05:30', 5, 1),
(16, 3, '2023-10-19T10:59:17.1399267+05:30', '2023-10-19T10:59:17.1399268+05:30', 6, 1),
(17, 4, '2023-10-19T10:59:17.1399269+05:30', '2023-10-19T10:59:17.1399269+05:30', 1, 1),
(18, 5, '2023-10-19T10:59:17.1399270+05:30', '2023-10-19T10:59:17.1399271+05:30', 1, 1),
(19, 6, '2023-10-19T10:59:17.1399272+05:30', '2023-10-19T10:59:17.1399272+05:30', 1, 1),
(20, 6, '2023-10-19T10:59:17.1399274+05:30', '2023-10-19T10:59:17.1399274+05:30', 2, 1),
(21, 6, '2023-10-19T10:59:17.1399275+05:30', '2023-10-19T10:59:17.1399276+05:30', 3, 1),
(22, 6, '2023-10-19T10:59:17.1399277+05:30', '2023-10-19T10:59:17.1399277+05:30', 4, 1),
(23, 7, '2023-10-19T10:59:17.1399278+05:30', '2023-10-19T10:59:17.1399279+05:30', 1, 1),
(24, 7, '2023-10-19T10:59:17.1399280+05:30', '2023-10-19T10:59:17.1399280+05:30', 2, 1),
(25, 7, '2023-10-19T10:59:17.1399281+05:30', '2023-10-19T10:59:17.1399282+05:30', 3, 1),
(26, 7, '2023-10-19T10:59:17.1399284+05:30', '2023-10-19T10:59:17.1399284+05:30', 4, 1),
(27, 8, '2023-10-19T10:59:17.1399285+05:30', '2023-10-19T10:59:17.1399286+05:30', 1, 1),
(28, 8, '2023-10-19T10:59:17.1399287+05:30', '2023-10-19T10:59:17.1399287+05:30', 2, 1),
(29, 8, '2023-10-19T10:59:17.1399288+05:30', '2023-10-19T10:59:17.1399289+05:30', 3, 1),
(30, 8, '2023-10-19T10:59:17.1399290+05:30', '2023-10-19T10:59:17.1399290+05:30', 4, 1),
(31, 6, '2023-10-19T10:59:17.1399291+05:30', '2023-10-19T10:59:17.1399292+05:30', 1, 1),
(32, 6, '2023-10-19T10:59:17.1399293+05:30', '2023-10-19T10:59:17.1399293+05:30', 2, 1),
(33, 6, '2023-10-19T10:59:17.1399294+05:30', '2023-10-19T10:59:17.1399295+05:30', 3, 1),
(34, 1, '2023-10-19T10:59:17.1399296+05:30', '2023-10-19T10:59:17.1399296+05:30', 1, 2),
(35, 3, '2023-10-19T10:59:17.1399297+05:30', '2023-10-19T10:59:17.1399298+05:30', 1, 2),
(36, 3, '2023-10-19T10:59:17.1399299+05:30', '2023-10-19T10:59:17.1399299+05:30', 2, 2),
(37, 3, '2023-10-19T10:59:17.1399300+05:30', '2023-10-19T10:59:17.1399301+05:30', 3, 2),
(38, 3, '2023-10-19T10:59:17.1399302+05:30', '2023-10-19T10:59:17.1399303+05:30', 4, 2),
(39, 3, '2023-10-19T10:59:17.1399304+05:30', '2023-10-19T10:59:17.1399304+05:30', 5, 2),
(40, 3, '2023-10-19T10:59:17.1399305+05:30', '2023-10-19T10:59:17.1399306+05:30', 6, 2),
(41, 2, '2023-10-19T10:59:17.1399307+05:30', '2023-10-19T10:59:17.1399307+05:30', 3, 2),
(42, 3, '2023-10-19T10:59:17.1399310+05:30', '2023-10-19T10:59:17.1399311+05:30', 6, 2);
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (43, 4, '2023-10-19T10:59:17.1399312+05:30', '2023-10-19T10:59:17.1399312+05:30', 1, 2),
(44, 5, '2023-10-19T10:59:17.1399313+05:30', '2023-10-19T10:59:17.1399314+05:30', 1, 2),
(45, 7, '2023-10-19T10:59:17.1399315+05:30', '2023-10-19T10:59:17.1399315+05:30', 1, 2),
(46, 7, '2023-10-19T10:59:17.1399316+05:30', '2023-10-19T10:59:17.1399317+05:30', 2, 2),
(47, 7, '2023-10-19T10:59:17.1399318+05:30', '2023-10-19T10:59:17.1399318+05:30', 3, 2),
(48, 7, '2023-10-19T10:59:17.1399319+05:30', '2023-10-19T10:59:17.1399320+05:30', 4, 2),
(49, 8, '2023-10-19T10:59:17.1399321+05:30', '2023-10-19T10:59:17.1399322+05:30', 1, 2),
(50, 8, '2023-10-19T10:59:17.1399323+05:30', '2023-10-19T10:59:17.1399323+05:30', 2, 2),
(51, 8, '2023-10-19T10:59:17.1399324+05:30', '2023-10-19T10:59:17.1399325+05:30', 3, 2),
(52, 8, '2023-10-19T10:59:17.1399325+05:30', '2023-10-19T10:59:17.1399326+05:30', 4, 2),
(53, 3, '2023-10-19T10:59:17.1399327+05:30', '2023-10-19T10:59:17.1399327+05:30', 3, 3),
(54, 5, '2023-10-19T10:59:17.1399328+05:30', '2023-10-19T10:59:17.1399329+05:30', 1, 3),
(55, 8, '2023-10-19T10:59:17.1399330+05:30', '2023-10-19T10:59:17.1399331+05:30', 1, 3),
(56, 8, '2023-10-19T10:59:17.1399332+05:30', '2023-10-19T10:59:17.1399332+05:30', 2, 3),
(57, 8, '2023-10-19T10:59:17.1399333+05:30', '2023-10-19T10:59:17.1399333+05:30', 3, 3),
(58, 8, '2023-10-19T10:59:17.1399334+05:30', '2023-10-19T10:59:17.1399335+05:30', 4, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OperationId', N'ActionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RoleActionPermission]'))
    SET IDENTITY_INSERT [RoleActionPermission] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Email', N'FirstName', N'LastName', N'Password', N'RefreshToken', N'RoleId', N'TokenExpiryTime') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([UserId], [Email], [FirstName], [LastName], [Password], [RefreshToken], [RoleId], [TokenExpiryTime])
VALUES (1, N'Spurtree@example.com', N'Spurtree', NULL, N'Spurtree@1234', NULL, 1, '0001-01-01T00:00:00.0000000'),
(2, N'Abhay@example.com', N'Abhay', NULL, N'Abhay@1234', NULL, 2, '0001-01-01T00:00:00.0000000'),
(3, N'Bhoomi@example.com', N'Bhoomi', NULL, N'Bhoomika@1234', NULL, 3, '0001-01-01T00:00:00.0000000');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Email', N'FirstName', N'LastName', N'Password', N'RefreshToken', N'RoleId', N'TokenExpiryTime') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrgUserId', N'CreatedTimestamp', N'ModifiedTimestamp', N'OrganizationId', N'Orgcode', N'UserId') AND [object_id] = OBJECT_ID(N'[OrganizationUsers]'))
    SET IDENTITY_INSERT [OrganizationUsers] ON;
INSERT INTO [OrganizationUsers] ([OrgUserId], [CreatedTimestamp], [ModifiedTimestamp], [OrganizationId], [Orgcode], [UserId])
VALUES (1, '2023-10-19T10:59:17.1399221+05:30', '2023-10-19T10:59:17.1399222+05:30', 1, N'CD01', 1),
(2, '2023-10-19T10:59:17.1399223+05:30', '2023-10-19T10:59:17.1399224+05:30', 1, N'CD01', 2),
(3, '2023-10-19T10:59:17.1399225+05:30', '2023-10-19T10:59:17.1399225+05:30', 3, N'DC02', 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrgUserId', N'CreatedTimestamp', N'ModifiedTimestamp', N'OrganizationId', N'Orgcode', N'UserId') AND [object_id] = OBJECT_ID(N'[OrganizationUsers]'))
    SET IDENTITY_INSERT [OrganizationUsers] OFF;
GO

CREATE INDEX [IX_Applications_OrganizationId] ON [Applications] ([OrganizationId]);
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
VALUES (N'20231019052917_10', N'7.0.11');
GO

COMMIT;
GO

