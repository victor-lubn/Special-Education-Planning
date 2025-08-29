CREATE   PROCEDURE [dbo].[AclEndUserCreate]
   @EndUserId int  
AS
BEGIN 

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'EndUser', @EndUserId, Educationer.Id
	From  [EndUser] eu
	INNER JOIN [Plan] pl on pl.EndUserId =  eu.Id
	INNER JOIN [Project] p on p.Id =  pl.ProjectId	
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  eu.Id =  @EndUserId



END

