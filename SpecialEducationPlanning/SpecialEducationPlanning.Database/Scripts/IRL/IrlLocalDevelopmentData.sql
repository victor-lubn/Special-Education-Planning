-- ========================================================================================================
-- ================================== LOCAL IRELAND DEVELOPMENT DATA ======================================
-- ========================================================================================================


-- Account numbers for builders in SAP Ireland
-- 0008000000
-- 0008000001
-- 0008000002
-- 0008000003

-- RELEASEINFO
INSERT [dbo].[ReleaseInfo] ([Title], [Subtitle], [Version], [FusionVersion], [DateTime], [Document], [DocumentPath]) VALUES (N'string', N'string', '1.0.0', NULL, CAST(N'2019-03-26T11:29:23.7680000' AS DateTime2), N'string', N'string')


-- GET IDs for roles
DECLARE @roleAdminId INT = (SELECT [Id] FROM [dbo].[Role] where [Name] = 'Admin')
DECLARE @roleEducationerId INT = (SELECT [Id] FROM [dbo].[Role] where [Name] = 'Educationer')

-- GET IDs for Aieps
DECLARE @Aiep1Id INT = (SELECT Id FROM [dbo].[Aiep] WHERE [AiepCode] = 'DY01') -- Ballyfermot
DECLARE @Aiep2Id INT = (SELECT Id FROM [dbo].[Aiep] WHERE [AiepCode] = 'DY02') -- Sandyford
DECLARE @Aiep1Code NVARCHAR(11) = (SELECT AiepCode FROM [dbo].[Aiep] WHERE [AiepCode] = 'DY01')
DECLARE @Aiep2Code NVARCHAR(11) = (SELECT AiepCode FROM [dbo].[Aiep] WHERE [AiepCode] = 'DY02')


-- ======= CREATE USERS
-- CREATE USER WITH FULL ACL ACCESS
INSERT [dbo].[User] ([FirstName], [Surname], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser], [UniqueIdentifier], [Discriminator], [AiepId], [ReleaseInfoId], [FullAclAccess], [Leaver], [DelegateToUserId]) VALUES ('roitdp', 'Support', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserCreate', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserUpdate', N'roitdp.support@hwdn.co.uk', N'Support', @Aiep1Id, NULL, 1, 0, NULL)
DECLARE @SupportId INT = (SELECT [Id] FROM [dbo].[User] where [UniqueIdentifier] = 'roitdp.support@hwdn.co.uk')

-- CREATE USER WITH ACCESS TO SANDYFORD Aiep (DY02)
INSERT [dbo].[User] ([FirstName], [Surname], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser], [UniqueIdentifier], [Discriminator], [AiepId], [ReleaseInfoId], [FullAclAccess], [Leaver], [DelegateToUserId]) VALUES ('roitdp', 'Educationer', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserCreate', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserUpdate', N'roitdp.Educationer@hwdn.co.uk', N'Educationer', @Aiep2Id, NULL, 0, 0, NULL)
DECLARE @EducationerId INT = (SELECT [Id] FROM [dbo].[User] where [UniqueIdentifier] = 'roitdp.Educationer@hwdn.co.uk')

-- CREATE USERROLE FOR ABOVE USERS
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (@SupportId, @roleAdminId)
INSERT [dbo].[UserRole] ([UserId], [RoleId]) VALUES (@EducationerId, @roleEducationerId)



-- ======= CREATE BUILDERS AND ASSIGN THEM TO AiepS
-- BUILDERS
INSERT [dbo].[Builder] ([AccountNumber], [TradingName], [Name], [CreatedDate], [Postcode], [Address1], [MobileNumber], [LandLineNumber], [Address0], [Address2], [Address3], [CreationUser], [UpdatedDate], [UpdateUser], [UniqueIdentifier], [Email]) VALUES (NULL, N'Cash Builder', N'Manual', CAST(N'2019-03-26T12:16:26.5109241' AS DateTime2), N'D02AF30', N'7', N'624954198', N'863586556', N'7 London Street Brighton', N'London Street', N'Brighton', N'vicente.pastor@aiep.com', CAST(N'2019-03-26T12:16:26.5109241' AS DateTime2), N'vicente.pastor@aiep.com', N'vpastobe@everis.com', N'vpastobe.Educationer@hwdn.co.uk')
INSERT [dbo].[Builder] ([AccountNumber], [TradingName], [Name], [CreatedDate], [Postcode], [Address1], [MobileNumber], [LandLineNumber], [Address0], [Address2], [Address3], [CreationUser], [UpdatedDate], [UpdateUser], [UniqueIdentifier], [Email]) VALUES ('1133555788', N'Credit Builder', N'Manual', CAST(N'2019-03-26T12:16:26.5109241' AS DateTime2), N'D02AF30', N'12', N'624954198', N'863586556', N'12 London Street Brighton', N'London Street', N'Brighton', N'D999TDP.Educationer@hwdn.co.uk', CAST(N'2019-03-26T12:16:26.5109241' AS DateTime2), N'D999TDP.Educationer@hwdn.co.uk', N'DZ99TDP.Educationer@hwdn.co.uk', N'DZ99TDP.Educationer@hwdn.co.uk')

-- VARIABLES
DECLARE @Builder1 INT = (SELECT Id FROM [dbo].[Builder] WHERE [TradingName] = 'Cash Builder')
DECLARE @Builder2 INT = (SELECT Id FROM [dbo].[Builder] WHERE [TradingName] = 'Credit Builder')
DECLARE @Builder1TradingName NVARCHAR(500) = (SELECT TradingName FROM [dbo].[Builder] WHERE [TradingName] = 'Cash Builder')
DECLARE @Builder2TradingName NVARCHAR(500) = (SELECT TradingName FROM [dbo].[Builder] WHERE [TradingName] = 'Credit Builder')
DECLARE @PlanCode1 NVARCHAR(11) = N'01234567891'
DECLARE @PlanCode2 NVARCHAR(11) = N'01234567892'
DECLARE @KitchenCatalogId INT = (SELECT Id FROM [dbo].[Catalog] WHERE [Name] = 'Kitchens')
DECLARE @LinearCatalogId INT = (SELECT Id FROM [dbo].[Catalog] WHERE [Name] = 'Linear')

-- BUILDEREducationERAiep - mapping builders to Aieps 
INSERT [dbo].[BuilderEducationerAiep] ([BuilderId], [AiepId]) VALUES (@Builder1, @Aiep1Id)
INSERT [dbo].[BuilderEducationerAiep] ([BuilderId], [AiepId]) VALUES (@Builder2, @Aiep2Id)


-- ======= CREATE END USER AND ASSIGN THEM TO Aiep
-- END USER
SET IDENTITY_INSERT [dbo].[EndUser] ON 
INSERT [dbo].[EndUser] ([Id], [Address0], [Address1], [Address2], [Address3], [Address4], [Address5], [Comment], [ContactEmail], [CountryCode], [FirstName], [LandLineNumber], [MobileNumber], [Postcode], [Surname], [TitleId], [UniqueIdentifier], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser], [FullName]) VALUES (1, N'28 Ravenshurst Avenue  LONDON', N'28 Ravenshurst Avenue', N' LONDON', NULL, NULL, NULL, NULL, N'richard.jones@aiep.com', NULL, N'Richard', NULL, NULL, N'D02AF30', N'Jones', 21, NULL, CAST(N'2021-08-12T12:57:23.2191548' AS DateTime2), N'roitdp.support@hwdn.co.uk', CAST(N'2021-08-12T12:57:23.2191548' AS DateTime2), N'roitdp.support@hwdn.co.uk', N'Richard Jones')
SET IDENTITY_INSERT [dbo].[EndUser] OFF
DECLARE @EndUser1Id INT = (SELECT Id FROM [dbo].[EndUser] WHERE [Id] = 1)

-- ENDUSERAiep - mapping end users to Aieps 
SET IDENTITY_INSERT [dbo].[EndUserAiep] ON 
INSERT [dbo].[EndUserAiep] ([Id], [EndUserId], [AiepId]) VALUES (1, @EndUser1Id, @Aiep2Id)
SET IDENTITY_INSERT [dbo].[EndUserAiep] OFF


-- ======= CREATE PROJECTS AND PLANS
-- PROJECT
SET IDENTITY_INSERT [dbo].[Project] ON 
INSERT [dbo].[Project] ([Id], [CodeProject], [AiepId], [KeyName], [SinglePlanProject], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser]) VALUES (1, FORMATMESSAGE(N'Project for plan %s', @PlanCode1), @Aiep1Id, NULL, 1, CAST(N'2021-08-12T12:57:21.9962581' AS DateTime2), N'roitdp.support@hwdn.co.uk', CAST(N'2021-08-12T12:57:21.9962581' AS DateTime2), N'roitdp.support@hwdn.co.uk')
INSERT [dbo].[Project] ([Id], [CodeProject], [AiepId], [KeyName], [SinglePlanProject], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser]) VALUES (2, FORMATMESSAGE(N'Project for plan %s', @PlanCode2), @Aiep2Id, NULL, 1, CAST(N'2021-08-12T12:57:21.9962581' AS DateTime2), N'roitdp.support@hwdn.co.uk', CAST(N'2021-08-12T12:57:21.9962581' AS DateTime2), N'roitdp.support@hwdn.co.uk')
SET IDENTITY_INSERT [dbo].[Project] OFF 
DECLARE @Project1Id INT = (SELECT Id FROM [dbo].[Project] WHERE [Id] = 1)
DECLARE @Project2Id INT = (SELECT Id FROM [dbo].[Project] WHERE [Id] = 2)

-- PLAN
SET IDENTITY_INSERT [dbo].[Plan] ON 
INSERT [dbo].[Plan] ([Id], [Title], [LastOpen], [CatalogId], [ProjectId] ,[KeyName], [Survey], [PlanCode], [EndUserId], [BuilderId], [EducationerId], [PlanState], [MasterVersionId], [IsStarred], [BuilderTradingName], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser], [PlanType], [CadFilePlanId], [PlanName], [OfflineSyncDate]) VALUES (1, NULL, CAST(N'2019-03-26T11:29:23.7680000' AS DateTime2), @KitchenCatalogId, @Project1Id, NULL, 1, @PlanCode1, NULL, @Builder1, @SupportId, 0, NULL, 0, @Builder1TradingName, CAST(N'2019-03-26T11:29:23.7680000' AS DateTime2), 'CADFile', CAST(N'2019-03-26T11:29:23.7680000' AS DateTime2), NULL, 'Private - Replacement', NULL, N'Test plan 1', NULL)
INSERT [dbo].[Plan] ([Id], [Title], [LastOpen], [CatalogId], [ProjectId] ,[KeyName], [Survey], [PlanCode], [EndUserId], [BuilderId], [EducationerId], [PlanState], [MasterVersionId], [IsStarred], [BuilderTradingName], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser], [PlanType], [CadFilePlanId], [PlanName], [OfflineSyncDate]) VALUES (2, NULL, CAST(N'2019-03-26T11:29:23.7680000' AS DateTime2), @LinearCatalogId, @Project2Id, NULL, 1, @PlanCode2, @EndUser1Id, @Builder2, @SupportId, 0, NULL, 0, @Builder2TradingName, CAST(N'2019-03-26T11:29:23.7680000' AS DateTime2), 'CADFile', CAST(N'2019-03-26T11:29:23.7680000' AS DateTime2), NULL, 'Private - Replacement', NULL, N'Test plan 2', NULL)
SET IDENTITY_INSERT [dbo].[Plan] OFF
DECLARE @Plan1Id INT = (SELECT Id FROM [dbo].[Plan] WHERE [Id] = 1)
DECLARE @Plan2Id INT = (SELECT Id FROM [dbo].[Plan] WHERE [Id] = 2)

-- VERSION
SET IDENTITY_INSERT [dbo].[Version] ON 
INSERT [dbo].[Version] ([Id], [KeyName], [PlanId], [Preview], [Rom], [PreviewPath], [RomPath], [VersionNumber], [VersionNotes], [Range], [CatalogId], [ExternalCode], [AiepCode], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser], [QuoteOrderNumber]) VALUES (1, NULL, 1, N'E:\WP765 - Trade Education Platform PSV\Scripts\Ready\07_DV_Modify Plan(Overwrite) (Fusion)_APIs_V1.2\preview_17081017278.jpeg', N'E:\WP765 - Trade Education Platform PSV\Scripts\Ready\07_DV_Modify Plan(Overwrite) (Fusion)_APIs_V1.2\17081017278_SAVE.Rom', N'Version/2019/05/6ec1d8de7a5a44a8b29f0b662d9d3ad4', N'Version/2019/05/4e4da106566344c8ad4d7776ab3740cc', 1, N'Version 1', N'  Allendale Antique White', @KitchenCatalogId, N'F3100000001', @Aiep1Code, CAST(N'2021-08-20T12:33:31.1323218' AS DateTime2), N'roitdp.support@hwdn.co.uk', CAST(N'2021-08-20T12:33:31.1323218' AS DateTime2), N'roitdp.support@hwdn.co.uk', N'')
INSERT [dbo].[Version] ([Id], [KeyName], [PlanId], [Preview], [Rom], [PreviewPath], [RomPath], [VersionNumber], [VersionNotes], [Range], [CatalogId], [ExternalCode], [AiepCode], [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser], [QuoteOrderNumber]) VALUES (2, NULL, 2, N'E:\WP765 - Trade Education Platform PSV\Scripts\Ready\07_DV_Modify Plan(Overwrite) (Fusion)_APIs_V1.2\preview_17081017278.jpeg', N'E:\WP765 - Trade Education Platform PSV\Scripts\Ready\07_DV_Modify Plan(Overwrite) (Fusion)_APIs_V1.2\17081017278_SAVE.Rom', N'Version/2019/05/6ec1d8de7a5a44a8b29f0b662d9d3ad4', N'Version/2019/05/4e4da106566344c8ad4d7776ab3740cc', 1, N'Version 1', N'  Willow', @LinearCatalogId, N'F4300000009', @Aiep2Code, CAST(N'2021-08-20T12:33:31.1323218' AS DateTime2), N'roitdp.support@hwdn.co.uk', CAST(N'2021-08-20T12:33:31.1323218' AS DateTime2), N'roitdp.support@hwdn.co.uk', N'')
SET IDENTITY_INSERT [dbo].[Version] OFF 
DECLARE @Version1Id INT = (SELECT Id FROM [dbo].[Version] WHERE [Id] = 1)
DECLARE @Version2Id INT = (SELECT Id FROM [dbo].[Version] WHERE [Id] = 2)

-- ADD VERSION ID TO PLANS
UPDATE [dbo].[Plan] SET MasterVersionId = @Version1Id WHERE Id = @Plan1Id
UPDATE [dbo].[Plan] SET MasterVersionId = @Version2Id WHERE Id = @Plan2Id



-- AVAILABLE USERS 
-- D999TDP.Educationer@hwdn.co.uk       
-- D999TDP.Manager@hwdn.co.uk       
-- D999TDP.FrontCounter@hwdn.co.uk      
-- D999TDP.Support@hwdn.co.uk      
-- DZ99TDP.Educationer@hwdn.co.uk      
-- DZ99TDP.Manager@hwdn.co.uk        
-- DZ99TDP.FrontCounter@hwdn.co.uk    
-- roitdp.support@hwdn.co.uk
-- pentestng1 - pentestng1@aiep.com 
-- pentestng2 - pentestng2@aiep.com 
-- pentestng3 вЂ“ pentestng3@aiep.com
-- roitdp.manager@hwdn.co.uk
-- roitdp.support@hwdn.co.uk
-- roitdp.Educationer@hwdn.co.uk
-- roitdp.hubuser@hwdn.co.uk







