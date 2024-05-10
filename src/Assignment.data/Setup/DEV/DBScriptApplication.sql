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
    [AppCode] nvarchar(12) NOT NULL,
    [Description] nvarchar(500) NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [ApplicationPhone] nvarchar(max) NOT NULL,
    [OrganizationId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [ClientId] nvarchar(max) NULL,
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
VALUES (1, N'raiden-settings', '2023-10-23T11:27:58.9197638+05:30', '2023-10-23T11:27:58.9197639+05:30'),
(2, N'manage-organization', '2023-10-23T11:27:58.9197640+05:30', '2023-10-23T11:27:58.9197641+05:30'),
(3, N'manage-application', '2023-10-23T11:27:58.9197642+05:30', '2023-10-23T11:27:58.9197643+05:30'),
(4, N'org-log', '2023-10-23T11:27:58.9197644+05:30', '2023-10-23T11:27:58.9197645+05:30'),
(5, N'app-log', '2023-10-23T11:27:58.9197645+05:30', '2023-10-23T11:27:58.9197646+05:30'),
(6, N'manage-superadmin', '2023-10-23T11:27:58.9197647+05:30', '2023-10-23T11:27:58.9197648+05:30'),
(7, N'manage-orgadmin', '2023-10-23T11:27:58.9197649+05:30', '2023-10-23T11:27:58.9197649+05:30'),
(8, N'manage-appadmin', '2023-10-23T11:27:58.9197650+05:30', '2023-10-23T11:27:58.9197651+05:30');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ActionId', N'ActionName', N'CreatedTimestamp', N'ModifiedTimestamp') AND [object_id] = OBJECT_ID(N'[Actions]'))
    SET IDENTITY_INSERT [Actions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrgCode', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] ON;
INSERT INTO [Organizations] ([OrganizationId], [CreatedTimestamp], [IsActive], [ModifiedTimestamp], [OrgCode], [OrganizationEmail], [OrganizationName], [OrganizationPhone])
VALUES (1, '2023-10-23T11:27:58.9197668+05:30', CAST(1 AS bit), '2023-10-23T11:27:58.9197669+05:30', N'CD01', N'orgA@example.com', N'Organization A', N'1234567890'),
(2, '2023-10-23T11:27:58.9197672+05:30', CAST(1 AS bit), '2023-10-23T11:27:58.9197673+05:30', N'AB01', N'orgB@example.com', N'Organization B', N'9876543210'),
(3, '2023-10-23T11:27:58.9197674+05:30', CAST(1 AS bit), '2023-10-23T11:27:58.9197675+05:30', N'DC02', N'orgC@example.com', N'Organization C', N'5555555555');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrganizationId', N'CreatedTimestamp', N'IsActive', N'ModifiedTimestamp', N'OrgCode', N'OrganizationEmail', N'OrganizationName', N'OrganizationPhone') AND [object_id] = OBJECT_ID(N'[Organizations]'))
    SET IDENTITY_INSERT [Organizations] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([PermissionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionName])
VALUES (1, '2023-10-23T11:27:58.9197582+05:30', '2023-10-23T11:27:58.9197584+05:30', N'view'),
(2, '2023-10-23T11:27:58.9197586+05:30', '2023-10-23T11:27:58.9197586+05:30', N'create'),
(3, '2023-10-23T11:27:58.9197587+05:30', '2023-10-23T11:27:58.9197588+05:30', N'edit'),
(4, '2023-10-23T11:27:58.9197589+05:30', '2023-10-23T11:27:58.9197590+05:30', N'delete'),
(5, '2023-10-23T11:27:58.9197591+05:30', '2023-10-23T11:27:58.9197592+05:30', N'enable'),
(6, '2023-10-23T11:27:58.9197593+05:30', '2023-10-23T11:27:58.9197593+05:30', N'disable');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionName') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [CreatedTimestamp], [ModifiedTimestamp], [RoleName])
VALUES (1, '2023-10-23T11:27:58.9197456+05:30', '2023-10-23T11:27:58.9197469+05:30', N'SuperAdmin'),
(2, '2023-10-23T11:27:58.9197470+05:30', '2023-10-23T11:27:58.9197471+05:30', N'OrgAdmin'),
(3, '2023-10-23T11:27:58.9197472+05:30', '2023-10-23T11:27:58.9197473+05:30', N'AppAdmin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedTimestamp', N'ModifiedTimestamp', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OperationId', N'ActionId', N'CreatedTimestamp', N'ModifiedTimestamp', N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RoleActionPermission]'))
    SET IDENTITY_INSERT [RoleActionPermission] ON;
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (1, 1, '2023-10-23T11:27:58.9197774+05:30', '2023-10-23T11:27:58.9197775+05:30', 1, 1),
(2, 1, '2023-10-23T11:27:58.9197777+05:30', '2023-10-23T11:27:58.9197778+05:30', 2, 1),
(3, 1, '2023-10-23T11:27:58.9197780+05:30', '2023-10-23T11:27:58.9197781+05:30', 3, 1),
(4, 1, '2023-10-23T11:27:58.9197782+05:30', '2023-10-23T11:27:58.9197783+05:30', 4, 1),
(5, 2, '2023-10-23T11:27:58.9197784+05:30', '2023-10-23T11:27:58.9197785+05:30', 1, 1),
(6, 2, '2023-10-23T11:27:58.9197786+05:30', '2023-10-23T11:27:58.9197787+05:30', 2, 1),
(7, 2, '2023-10-23T11:27:58.9197789+05:30', '2023-10-23T11:27:58.9197790+05:30', 3, 1),
(8, 2, '2023-10-23T11:27:58.9197791+05:30', '2023-10-23T11:27:58.9197792+05:30', 4, 1),
(9, 2, '2023-10-23T11:27:58.9197794+05:30', '2023-10-23T11:27:58.9197795+05:30', 5, 1),
(10, 2, '2023-10-23T11:27:58.9197797+05:30', '2023-10-23T11:27:58.9197797+05:30', 6, 1),
(11, 3, '2023-10-23T11:27:58.9197799+05:30', '2023-10-23T11:27:58.9197800+05:30', 1, 1),
(12, 3, '2023-10-23T11:27:58.9197801+05:30', '2023-10-23T11:27:58.9197802+05:30', 2, 1),
(13, 3, '2023-10-23T11:27:58.9197803+05:30', '2023-10-23T11:27:58.9197804+05:30', 3, 1),
(14, 3, '2023-10-23T11:27:58.9197805+05:30', '2023-10-23T11:27:58.9197806+05:30', 4, 1),
(15, 3, '2023-10-23T11:27:58.9197808+05:30', '2023-10-23T11:27:58.9197808+05:30', 5, 1),
(16, 3, '2023-10-23T11:27:58.9197810+05:30', '2023-10-23T11:27:58.9197810+05:30', 6, 1),
(17, 4, '2023-10-23T11:27:58.9197812+05:30', '2023-10-23T11:27:58.9197813+05:30', 1, 1),
(18, 5, '2023-10-23T11:27:58.9197814+05:30', '2023-10-23T11:27:58.9197815+05:30', 1, 1),
(19, 6, '2023-10-23T11:27:58.9197816+05:30', '2023-10-23T11:27:58.9197817+05:30', 1, 1),
(20, 6, '2023-10-23T11:27:58.9197818+05:30', '2023-10-23T11:27:58.9197819+05:30', 2, 1),
(21, 6, '2023-10-23T11:27:58.9197821+05:30', '2023-10-23T11:27:58.9197822+05:30', 3, 1),
(22, 6, '2023-10-23T11:27:58.9197823+05:30', '2023-10-23T11:27:58.9197824+05:30', 4, 1),
(23, 7, '2023-10-23T11:27:58.9197826+05:30', '2023-10-23T11:27:58.9197826+05:30', 1, 1),
(24, 7, '2023-10-23T11:27:58.9197828+05:30', '2023-10-23T11:27:58.9197829+05:30', 2, 1),
(25, 7, '2023-10-23T11:27:58.9197830+05:30', '2023-10-23T11:27:58.9197831+05:30', 3, 1),
(26, 7, '2023-10-23T11:27:58.9197832+05:30', '2023-10-23T11:27:58.9197833+05:30', 4, 1),
(27, 8, '2023-10-23T11:27:58.9197834+05:30', '2023-10-23T11:27:58.9197835+05:30', 1, 1),
(28, 8, '2023-10-23T11:27:58.9197836+05:30', '2023-10-23T11:27:58.9197837+05:30', 2, 1),
(29, 8, '2023-10-23T11:27:58.9197838+05:30', '2023-10-23T11:27:58.9197839+05:30', 3, 1),
(30, 8, '2023-10-23T11:27:58.9197841+05:30', '2023-10-23T11:27:58.9197841+05:30', 4, 1),
(31, 6, '2023-10-23T11:27:58.9197843+05:30', '2023-10-23T11:27:58.9197843+05:30', 1, 1),
(32, 6, '2023-10-23T11:27:58.9197846+05:30', '2023-10-23T11:27:58.9197846+05:30', 2, 1),
(33, 6, '2023-10-23T11:27:58.9197848+05:30', '2023-10-23T11:27:58.9197849+05:30', 3, 1),
(34, 1, '2023-10-23T11:27:58.9197850+05:30', '2023-10-23T11:27:58.9197851+05:30', 1, 2),
(35, 3, '2023-10-23T11:27:58.9197852+05:30', '2023-10-23T11:27:58.9197853+05:30', 1, 2),
(36, 3, '2023-10-23T11:27:58.9197854+05:30', '2023-10-23T11:27:58.9197855+05:30', 2, 2),
(37, 3, '2023-10-23T11:27:58.9197856+05:30', '2023-10-23T11:27:58.9197857+05:30', 3, 2),
(38, 3, '2023-10-23T11:27:58.9197859+05:30', '2023-10-23T11:27:58.9197859+05:30', 4, 2),
(39, 3, '2023-10-23T11:27:58.9197861+05:30', '2023-10-23T11:27:58.9197861+05:30', 5, 2),
(40, 3, '2023-10-23T11:27:58.9197863+05:30', '2023-10-23T11:27:58.9197864+05:30', 6, 2),
(41, 2, '2023-10-23T11:27:58.9197865+05:30', '2023-10-23T11:27:58.9197866+05:30', 3, 2),
(42, 3, '2023-10-23T11:27:58.9197867+05:30', '2023-10-23T11:27:58.9197868+05:30', 6, 2);
INSERT INTO [RoleActionPermission] ([OperationId], [ActionId], [CreatedTimestamp], [ModifiedTimestamp], [PermissionId], [RoleId])
VALUES (43, 4, '2023-10-23T11:27:58.9197870+05:30', '2023-10-23T11:27:58.9197871+05:30', 1, 2),
(44, 5, '2023-10-23T11:27:58.9197872+05:30', '2023-10-23T11:27:58.9197873+05:30', 1, 2),
(45, 7, '2023-10-23T11:27:58.9197874+05:30', '2023-10-23T11:27:58.9197875+05:30', 1, 2),
(46, 7, '2023-10-23T11:27:58.9197876+05:30', '2023-10-23T11:27:58.9197877+05:30', 2, 2),
(47, 7, '2023-10-23T11:27:58.9197879+05:30', '2023-10-23T11:27:58.9197879+05:30', 3, 2),
(48, 7, '2023-10-23T11:27:58.9197881+05:30', '2023-10-23T11:27:58.9197881+05:30', 4, 2),
(49, 8, '2023-10-23T11:27:58.9197883+05:30', '2023-10-23T11:27:58.9197884+05:30', 1, 2),
(50, 8, '2023-10-23T11:27:58.9197885+05:30', '2023-10-23T11:27:58.9197886+05:30', 2, 2),
(51, 8, '2023-10-23T11:27:58.9197887+05:30', '2023-10-23T11:27:58.9197888+05:30', 3, 2),
(52, 8, '2023-10-23T11:27:58.9197889+05:30', '2023-10-23T11:27:58.9197890+05:30', 4, 2),
(53, 3, '2023-10-23T11:27:58.9197891+05:30', '2023-10-23T11:27:58.9197892+05:30', 3, 3),
(54, 5, '2023-10-23T11:27:58.9197893+05:30', '2023-10-23T11:27:58.9197894+05:30', 1, 3),
(55, 8, '2023-10-23T11:27:58.9197897+05:30', '2023-10-23T11:27:58.9197898+05:30', 1, 3),
(56, 8, '2023-10-23T11:27:58.9197900+05:30', '2023-10-23T11:27:58.9197900+05:30', 2, 3),
(57, 8, '2023-10-23T11:27:58.9197902+05:30', '2023-10-23T11:27:58.9197903+05:30', 3, 3),
(58, 8, '2023-10-23T11:27:58.9197904+05:30', '2023-10-23T11:27:58.9197905+05:30', 4, 3);
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
VALUES (1, '2023-10-23T11:27:58.9197692+05:30', '2023-10-23T11:27:58.9197693+05:30', 1, N'CD01', 1),
(2, '2023-10-23T11:27:58.9197695+05:30', '2023-10-23T11:27:58.9197696+05:30', 1, N'CD01', 2),
(3, '2023-10-23T11:27:58.9197697+05:30', '2023-10-23T11:27:58.9197698+05:30', 3, N'DC02', 3);
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
VALUES (N'20231023055759_test55', N'7.0.11');
GO

COMMIT;
GO

