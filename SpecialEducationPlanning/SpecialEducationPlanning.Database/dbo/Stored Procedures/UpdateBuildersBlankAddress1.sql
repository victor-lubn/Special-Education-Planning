CREATE   PROCEDURE [dbo].[UpdateBuildersBlankAddress1]
  
AS
BEGIN 	
	UPDATE Builder SET Address1=(CASE WHEN (LEN(RTRIM(LTRIM(Address0))) = 0 OR Address0 is null) 
	THEN 'N/A' ELSE Address0 END)
	WHERE Address1 IS NULL OR LEN(RTRIM(LTRIM(Address1)))=0

	SELECT @@ROWCOUNT
END