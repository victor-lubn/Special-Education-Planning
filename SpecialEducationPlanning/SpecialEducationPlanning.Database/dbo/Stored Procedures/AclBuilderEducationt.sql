CREATE   PROCEDURE [dbo].[AclBuilderAiep]
   @BuilderId int,
   @AiepId int
AS
BEGIN 	

	Delete a From Acl a
	INNER JOIN BuilderEducationerAiep bd on bd.BuilderId =  a.EntityId
	INNER JOIN [User] Educationer on Educationer.AiepId =  bd.AiepId
	Where  bd.BuilderId = @BuilderId and bd.AiepId = @AiepId and a.EntityType = 'Builder' and a.UserId IN (
		SELECT u.Id
		FROM [User] u 
		WHERE u.AiepId = @AiepId)
	
	INSERT INTO Acl (EntityType, EntityId, UserId) 
	SELECT DISTINCT 'Builder', @BuilderId, Educationer.Id
	From  BuilderEducationerAiep bd
	INNER JOIN [User] Educationer on Educationer.AiepId =  bd.AiepId
	Where  BuilderId = @BuilderId and Educationer.AiepId =  @AiepId 

END

