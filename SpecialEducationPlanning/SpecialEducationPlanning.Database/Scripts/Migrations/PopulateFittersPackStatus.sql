
SET NOCOUNT ON;

-- Step 1:Populate FittersPackStatus
IF NOT EXISTS(SELECT * FROM [dbo].[FittersPackStatus])
BEGIN
  SET IDENTITY_INSERT [dbo].[FittersPackStatus] ON 
  INSERT [dbo].[FittersPackStatus] ([Id], [Name]) VALUES (1,'Queued')
  INSERT [dbo].[FittersPackStatus] ([Id], [Name]) VALUES (2,'Processing')
  INSERT [dbo].[FittersPackStatus] ([Id], [Name]) VALUES (3,'Completed')
  INSERT [dbo].[FittersPackStatus] ([Id], [Name]) VALUES (4,'Failed')
  INSERT [dbo].[FittersPackStatus] ([Id], [Name]) VALUES (5,'OverDueQueued')
  INSERT [dbo].[FittersPackStatus] ([Id], [Name]) VALUES (6,'RetryFailed')
  INSERT [dbo].[FittersPackStatus] ([Id], [Name]) VALUES (7,'OverDueFailed')
  SET IDENTITY_INSERT [dbo].[FittersPackStatus] OFF
END

PRINT 'Script `PopulateFittersPackStatus` execution completed.';
