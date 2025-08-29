CREATE TABLE [dbo].[Builder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountNumber] [nvarchar](100) NULL,
	[Address0] [nvarchar](500) NULL,
	[Address1] [nvarchar](100) NULL,
	[Address2] [nvarchar](100) NULL,
	[Address3] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[LandLineNumber] [nvarchar](30) NULL,
	[MobileNumber] [nvarchar](30) NULL,
	[Name] [nvarchar](max) NULL,
	[Notes] [nvarchar](500) NULL,
	[Postcode] [nvarchar](20) NULL,
	[TradingName] [nvarchar](100) NOT NULL,
	[UniqueIdentifier] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreationUser] [nvarchar](100) NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[UpdateUser] [nvarchar](100) NULL,
	[SAPAccountStatus] [nvarchar](100) NULL,
	[OwningAiepCode] [nvarchar](20) NULL,
	[OwningAiepName] [nvarchar](100) NULL, 
    [BuilderStatus] INT NOT NULL,
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Builder] ADD  CONSTRAINT [PK_Builder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Builder] ADD  CONSTRAINT [DF_Builder_BuilderStatus]  DEFAULT ((0)) FOR [BuilderStatus]
GO
CREATE NONCLUSTERED INDEX [IX_Builder_AccountNumber] ON [dbo].[Builder]
(
	[AccountNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Builder_Address1_Postcode] ON [dbo].[Builder]
(
	[Address1] ASC,
	[Postcode] ASC
)
INCLUDE([AccountNumber],[Address0],[Address2],[Address3],[CreatedDate],[CreationUser],[Email],[LandLineNumber],[MobileNumber],[Name],[Notes],[TradingName],[UniqueIdentifier],[UpdatedDate],[UpdateUser]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Builder_Postcode] ON [dbo].[Builder]
(
	[Postcode] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Builder_TradingName_Postcode_Address1_AccountNumber] ON [dbo].[Builder]
(
	[TradingName] ASC,
	[Postcode] ASC,
	[Address1] ASC,
	[AccountNumber] ASC
)
WHERE ([Postcode] IS NOT NULL AND [Address1] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Builder_UpdatedDate] ON [dbo].[Builder]
(
	[UpdatedDate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

