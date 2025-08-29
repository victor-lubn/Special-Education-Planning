-- ========================================================================================================
-- ======================================= STATIC IRELAND DATA ============================================
-- ========================================================================================================

-- fixed ID (master data)
-- CATALOG
SET IDENTITY_INSERT [dbo].[Catalog] ON 
INSERT [dbo].[Catalog] ([Id], [Name], [Code], [Range], [Enabled]) VALUES (1, N'Kitchens', N'1458299216', NULL, 1)
INSERT [dbo].[Catalog] ([Id], [Name], [Code], [Range], [Enabled]) VALUES (2, N'Linear', N'1497616168', NULL, 1)
INSERT [dbo].[Catalog] ([Id], [Name], [Code], [Range], [Enabled]) VALUES (15, N'InFrame', N'1617791208', NULL, 1)
SET IDENTITY_INSERT [dbo].[Catalog] OFF

-- TITLE
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (1,N'MR')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (2,N'MRS')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (3,N'MISS')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (4,N'MS')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (5,N'MR & MRS')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (6,N'ADM')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (7,N'BT')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (8,N'CMDR')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (9,N'DAME')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (10,N'BRIG')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (11,N'CAPT')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (12,N'COL')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (13,N'DR')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (14,N'DUKE')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (15,N'EARL')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (16,N'FLDM')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (17,N'FTHR')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (18,N'GEN')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (19,N'HON')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (20,N'LADY')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (21,N'LORD')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (22,N'LT')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (23,N'MAJ')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (24,N'MGEN')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (25,N'MSRS')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (26,N'PROF')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (27,N'RADM')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (28,N'REV')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (29,N'RSM')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (30,N'SGT')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (31,N'SIR')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (32,N'SIST')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (33,N'SQDN')
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (34,N'MR & MR');
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (35,N'MRS & MRS');
INSERT [dbo].[Title] ([Id], [TitleName]) VALUES (36,N'MX');

-- COUNTRY
INSERT [dbo].[Country] ([KeyName]) VALUES (N'Republic of Ireland')
DECLARE @CountryId INT = (SELECT Id FROM [dbo].[Country] WHERE [KeyName] = 'Republic of Ireland')

--REGIONS
INSERT [dbo].[Region] ([CountryId], [KeyName]) VALUES (@CountryId, N'Republic of Ireland')
DECLARE @region1 INT = (SELECT Id FROM [dbo].[Region] WHERE [KeyName] = 'Republic of Ireland')

--AREAS
INSERT [dbo].[Area] ([RegionId], [KeyName]) VALUES (@region1, N'Republic of Ireland')
DECLARE @area1 INT = (SELECT Id FROM [dbo].[Area] WHERE [KeyName] = 'Republic of Ireland')

-- AiepS
INSERT [dbo].[Aiep] ([ReleaseInfoId], [AiepCode], [AreaId], [Name], [Email], [Address1], [Address2], [Address3], [Address4], [Address5], [Address6], [Postcode], [PhoneNumber], [FaxNumber], [IpAddress], [MediaServer], [HtmlEmail], [DownloadSpeed], [DownloadLimit], [ManagerId] , [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser]) VALUES (NULL, N'DY01', @area1, N'Ballyfermot', N'Ballyfermot@aiep.com', N'Unit 9', N'Westlink Industrial Estate', N'Kylemore Road', NULL, N'Dublin 10', NULL, N'D10H585', N'Unknown', NULL, NULL, NULL, 0, 0, 0, NULL, CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserCreate', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserUpdate')
INSERT [dbo].[Aiep] ([ReleaseInfoId], [AiepCode], [AreaId], [Name], [Email], [Address1], [Address2], [Address3], [Address4], [Address5], [Address6], [Postcode], [PhoneNumber], [FaxNumber], [IpAddress], [MediaServer], [HtmlEmail], [DownloadSpeed], [DownloadLimit], [ManagerId] , [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser]) VALUES (NULL, N'DY02', @area1, N'Sandyford', N'Sandyford@aiep.com', N'Unit 25', N'Stillorgan Business Park', N'Spruce Avenue', N'Stillorgan', N'County Dublin', NULL, N'A94VE03', N'Unknown', NULL, NULL, NULL, 0, 0, 0, NULL, CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserCreate', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserUpdate')
INSERT [dbo].[Aiep] ([ReleaseInfoId], [AiepCode], [AreaId], [Name], [Email], [Address1], [Address2], [Address3], [Address4], [Address5], [Address6], [Postcode], [PhoneNumber], [FaxNumber], [IpAddress], [MediaServer], [HtmlEmail], [DownloadSpeed], [DownloadLimit], [ManagerId] , [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser]) VALUES (NULL, N'DY03', @area1, N'Swords', N'Swords@aiep.com', N'Unit 4a', N'Feltrim Business Park', N'Feltrim Road', N'Swords', N'County Dublin', NULL, N'K67AY90', N'Unknown', NULL, NULL, NULL, 0, 0, 0, NULL, CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserCreate', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserUpdate')
INSERT [dbo].[Aiep] ([ReleaseInfoId], [AiepCode], [AreaId], [Name], [Email], [Address1], [Address2], [Address3], [Address4], [Address5], [Address6], [Postcode], [PhoneNumber], [FaxNumber], [IpAddress], [MediaServer], [HtmlEmail], [DownloadSpeed], [DownloadLimit], [ManagerId] , [CreatedDate], [CreationUser], [UpdatedDate], [UpdateUser]) VALUES (NULL, N'DY05', @area1, N'Bray', N'Bray@aiep.com', N'Unit 1 & 2', NULL, N'Boghall Road', N'Bray', N'County Wicklow', NULL, N'A98T2N8', N'Unknown', NULL, NULL, NULL, 0, 0, 0, NULL, CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserCreate', CAST(N'2019-03-26T08:59:15.5633333' AS DateTime2), N'testUserUpdate')

UPDATE [dbo].[Aiep] SET [Postcode] = REPLACE(LTRIM(RTRIM([Postcode])), ' ', '')



