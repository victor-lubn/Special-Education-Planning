CREATE   PROCEDURE [dbo].[AclProjectCreate]
   @ProjectId int  
AS
BEGIN 

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Project', @ProjectId, Educationer.Id
	From  Project p
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  p.Id = @ProjectId

END

