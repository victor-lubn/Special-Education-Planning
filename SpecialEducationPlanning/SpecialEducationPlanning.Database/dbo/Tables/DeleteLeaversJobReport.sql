CREATE TABLE [dbo].[DeleteLeaversJobReport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NumberOfLeaversDeleted] [int] NOT NULL,
	[NumberOfRemainingLeavers] [int] NOT NULL,
	[Comments] [nvarchar](200) NULL,
	[ReportedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_DeleteLeaversJobReport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DeleteLeaversJobReport] ADD  CONSTRAINT [DF_DeleteLeaversJobReport_NumberOfLeaversDeleted]  DEFAULT ((0)) FOR [NumberOfLeaversDeleted]
GO

ALTER TABLE [dbo].[DeleteLeaversJobReport] ADD  CONSTRAINT [DF_DeleteLeaversJobReport_NumberOfRemainingLeavers]  DEFAULT ((0)) FOR [NumberOfRemainingLeavers]
GO

ALTER TABLE [dbo].[DeleteLeaversJobReport] ADD  CONSTRAINT [DF_DeleteLeaversJobReport_ReportedDate]  DEFAULT (getdate()) FOR [ReportedDate]
GO


