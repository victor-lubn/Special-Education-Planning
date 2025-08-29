
SET NOCOUNT ON;

-- Step 1:Populate EducationToolOrigin
IF NOT EXISTS(SELECT * FROM [dbo].[EducationToolOrigin])
BEGIN
  SET IDENTITY_INSERT [dbo].[EducationToolOrigin] ON 
  INSERT [dbo].[EducationToolOrigin] ([Id], [Name]) VALUES (1,'Fusion')
  INSERT [dbo].[EducationToolOrigin] ([Id], [Name]) VALUES (2,'3DC')
  SET IDENTITY_INSERT [dbo].[EducationToolOrigin] OFF
END

DECLARE @BatchSize INT = 1000;
DECLARE @RowsCount INT = 0;
DECLARE @RowsAffected INT = 1;
DECLARE @BatchCount INT = 0;
DECLARE @Message NVARCHAR(250);
DECLARE @FusionEducationToolOriginId INT
DECLARE @3DCEducationToolOriginId INT

SELECT @FusionEducationToolOriginId = Id FROM [dbo].[EducationToolOrigin] WHERE [Name] = 'Fusion';
SELECT @3DCEducationToolOriginId = Id FROM [dbo].[EducationToolOrigin] WHERE [Name] = '3DC';


-- Step 2: Batch Update Plan EducationToolOriginId
WHILE @RowsAffected > 0
BEGIN
  UPDATE TOP (@BatchSize) [dbo].[Plan]
  SET EducationToolOriginId = @FusionEducationToolOriginId
  WHERE EducationToolOriginId IS NULL;

  SET @RowsAffected = @@ROWCOUNT;

  SET @RowsCount = @RowsCount + @RowsAffected;
  SET @BatchCount = @BatchCount + 1;

  SELECT @Message = 'Batch ' + CAST(@BatchCount AS VARCHAR(10)) + ': Updated ' + CAST(@RowsCount AS VARCHAR(10));
  RAISERROR (@Message, 0, 1) WITH NOWAIT;

  WAITFOR DELAY '00:00:00.2';
END;

PRINT 'Finished updating Plan table EducationToolOriginId column. Total rows updated: ' + CAST(@RowsCount AS VARCHAR(10));


-- Step 3: Update Catalog EducationToolOriginId
UPDATE [dbo].[Catalog]
  SET EducationToolOriginId = @FusionEducationToolOriginId
  WHERE EducationToolOriginId IS NULL;

PRINT 'Finished updating Catalog table EducationToolOriginId column. Total rows updated: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- Step 4: Populate 3DC Catalogs into [Catalog]
INSERT [dbo].[Catalog]([Name], [Enabled], [Code], [EducationToolOriginId])
SELECT [Source].[Name], [Source].[Enabled], [Source].[Code], @3DCEducationToolOriginId
FROM [dbo].[Catalog] AS [Source]
WHERE
  [Source].[EducationToolOriginId] = @FusionEducationToolOriginId
  AND NOT EXISTS (
    SELECT 1 
	FROM [dbo].[Catalog] AS [Target] 
	WHERE [Target].[Name] = [Source].[Name] AND [Target].[EducationToolOriginId] = @3DCEducationToolOriginId
  );
 
PRINT 'Finished population Catalog table by 3DC Catalogs. Total rows added: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

PRINT 'Script execution completed.';

