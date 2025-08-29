-- ============================================== HOW TO USE ===============================================
-- In order to populate a new EducationView database, this script must be run to insert the static data that
-- is used for all countries. 

-- To add country specific data, navigate to the country specific folder in this project ->
-- \SpecialEducationPlanning
.Database\Scripts\Country (e.g. UK, Ireland etc)
-- Run the static data script for the specific country found in the folder.

-- When doing local development, also run the LocalDevelopmentData.sql script in the country specific folder
-- to insert dummy data in the database. 

--IMPORTANT: 
-- After running LocalDevelopmentData.sql, you must update the ACL table to ensure users have the correct 
-- access. To do this, select the users from the Users table. Each user has a AiepID. For each different 
-- Aiep ID in the AiepID column, run the stored procedure called 'AclAiep'. To run the stored procedure, 
-- right click on it and select 'Execute Stored Procedure...' The stored procedure takes a Aiep ID as a 
-- parameter. Run the stored procedure for each Aiep ID in the AiepID column of the Users table, each 
-- time passing the Aiep ID value as the parameter to the stored procedure

-- ========================================================================================================
-- ======================================== GLOBAL STATIC DATA ============================================
-- ========================================================================================================

-- fixed ID (master data)
-- ROLES
INSERT [dbo].[Role] ([Name]) VALUES (N'Admin')
INSERT [dbo].[Role] ([Name]) VALUES (N'Educationer')
INSERT [dbo].[Role] ([Name]) VALUES (N'AiepManager')
INSERT [dbo].[Role] ([Name]) VALUES (N'HubUser')

-- GET IDs for roles
DECLARE @roleAdminId INT = (SELECT [Id] FROM [dbo].[Role] where [Name] = 'Admin')
DECLARE @roleManagerId INT = (SELECT [Id] FROM [dbo].[Role] where [Name] = 'AiepManager')
DECLARE @roleEducationerId INT = (SELECT [Id] FROM [dbo].[Role] where [Name] = 'Educationer')
DECLARE @roleHubUserId INT = (SELECT [Id] FROM [dbo].[Role] where [Name] = 'HubUser')

-- PERMISSIONS
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Structure_Management', N'Structure_Management')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Data_Management', N'Data_Management')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Report_Request', N'Report_Request')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Log_Request', N'Log_Request')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Plan_Management', N'Plan_Management')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Plan_Create', N'Plan_Create')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Plan_Modify', N'Plan_Modify')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Plan_Delete', N'Plan_Delete')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Plan_Publish', N'Plan_Publish')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Plan_Comment', N'Plan_Comment')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Comment_Management', N'Comment_Management')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'User_Management', N'User_Management')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Role_Management', N'Role_Management')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Hub_Management', N'Hub_Management')
INSERT [dbo].[Permission] ([Name], [DescriptionCode]) VALUES (N'Project_Management', N'Project_Management')

-- GET IDs for permissions
DECLARE @permStructureManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Structure_Management')
DECLARE @permDataManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Data_Management')
DECLARE @permReportRequestId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Report_Request')
DECLARE @permLogRequestId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Log_Request')
DECLARE @permPlanManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Plan_Management')
DECLARE @permPlanCreateId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Plan_Create')
DECLARE @permPlanModifyId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Plan_Modify')
DECLARE @permPlanDeleteId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Plan_Delete')
DECLARE @permPlanPublishId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Plan_Publish')
DECLARE @permPlanCommentId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Plan_Comment')
DECLARE @permCommentManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Comment_Management')
DECLARE @permUserManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'User_Management')
DECLARE @permRoleManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Role_Management')
DECLARE @permHubManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Hub_Management')
DECLARE @permProjectManagementId INT = (SELECT [Id] FROM [dbo].[Permission] WHERE [Name] = 'Project_Management')

-- PERMISSIONROLE 
-- Admin
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permStructureManagementId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permDataManagementId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permReportRequestId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permLogRequestId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanManagementId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCreateId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanModifyId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanDeleteId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanPublishId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCommentId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permCommentManagementId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permUserManagementId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permRoleManagementId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permHubManagementId, @roleAdminId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permProjectManagementId, @roleAdminId)
-- Aiep Manager
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCreateId, @roleManagerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanModifyId, @roleManagerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanDeleteId, @roleManagerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanPublishId, @roleManagerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCommentId, @roleManagerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permCommentManagementId, @roleManagerId)
-- Educationer
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCreateId, @roleEducationerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanModifyId, @roleEducationerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanDeleteId, @roleEducationerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanPublishId, @roleEducationerId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCommentId, @roleEducationerId)
-- Hub User
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permDataManagementId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanManagementId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCreateId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanModifyId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanDeleteId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanPublishId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permPlanCommentId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permHubManagementId, @roleHubUserId)
INSERT [dbo].[PermissionRole] ([PermissionId], [RoleId]) VALUES (@permProjectManagementId, @roleHubUserId)

-- SOUNDTRACK
SET IDENTITY_INSERT [dbo].[Soundtrack] ON 
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (1, CAST(N'2019-03-26T08:59:15.5066667' AS DateTime2), N'None', N'00')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (2, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Backgrounds', N'01')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (3, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Fresh New Start', N'02')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (4, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Song of the Forest', N'03')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (5, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Ascension', N'04')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (6, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Loose', N'05')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (7, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Positive Reinforcement', N'06')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (8, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'A Dream For You', N'07')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (9, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'In The Sky', N'08')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (10, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Raise The Bar', N'09')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (11, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Stars at Night', N'10')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (12, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Bright Shining Love', N'11')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (13, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Happy Vibes', N'12')
INSERT [dbo].[Soundtrack] ([Id], [CreatedDate], [Display], [Code]) VALUES (14, CAST(N'2019-03-26T08:59:15.5100000' AS DateTime2), N'Memories of Rain', N'13')
SET IDENTITY_INSERT [dbo].[Soundtrack] OFF

--SORTINGFILTERING - used in dropdowns
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Plan', N'PlanCode', N'Plan ID')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Plan', N'EndUser.Surname', N'End Client')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Plan', N'BuilderTradingName', N'Trading Name')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Plan', N'Educationer.Surname', N'Educationer')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Plan', N'CadFilePlanId', N'CADfile Plan ID')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Plan', N'MasterVersionId', N'Master Version Id')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Plan', N'UpdatedDate', N'Last Update')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Builder', N'AccountNumber', 'Account Number')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Builder', N'TradingName', N'Trading Name')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Builder', N'Address1', N'Address')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Builder', N'Postcode', N'Postcode')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Builder', N'MobileNumber', N'Mobile Number')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'SystemLog', N'Level' , N'Level')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'SystemLog', N'Message', N'Message')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'SystemLog', N'MessageTemplate', N'Message Template')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'SystemLog', N'Exception', N'Exception Type')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'SystemLog', N'Properties', N'Properties')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'SystemLog', N'TimeStamp', N'Date')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ActionLogs', N'ActionType', N'Action')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ActionLogs', N'User', N'User')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ActionLogs', N'Date', N'Date')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ActionLogs', N'EntityName', N'Belongs to')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ActionLogs', N'EntityValue', N'Value')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ReleaseInfo', N'Title', N'Title')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ReleaseInfo', N'Version', N'DV Version')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ReleaseInfo', N'FusionVersion', N'Fusion Version')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ReleaseInfo', N'DateTime', N'Date')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ArchivedPlan', N'PlanCode', N'Plan Number')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ArchivedPlan', N'EndUser.FirstName', N'End Client')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ArchivedPlan', N'BuilderTradingName', N'Trading Name')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'ArchivedPlan', N'UpdatedDate', N'Archived On')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Country', N'KeyName', N'Name')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Country', N'RegionsCount', N'Regions')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Region', N'KeyName', N'Region')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Region', N'AreasCount', N'Areas')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Area', N'KeyName', N'Area')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Area', N'AiepCount', N'Aieps')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Aiep', N'AiepCode', N'Aiep Code')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Aiep', N'Name', N'Name')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Aiep', N'Area.KeyName', N'Area')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Aiep', N'UpdatedDate', N'Last Update')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'Role', N'Name', N'Role')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'User', N'FirstName', N'Name')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'User', N'UniqueIdentifier', N'UPN')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'User', N'Aiep.AiepCode', N'Aiep Code')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'User', N'Aiep.Name', N'Aiep Name')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'BuilderPlan', N'Id', N'Plan ID')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'BuilderPlan', N'EndUser.Surname', N'End Client')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'BuilderPlan', N'Educationer.Surname', N'Educationer')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'BuilderPlan', N'cadFilePlanId', N'CADfile Plan ID')
INSERT [dbo].[SortingFiltering] ([EntityType], [PropertyName], [PropertyText]) VALUES (N'BuilderPlan', N'updatedDate', N'Last Update')

