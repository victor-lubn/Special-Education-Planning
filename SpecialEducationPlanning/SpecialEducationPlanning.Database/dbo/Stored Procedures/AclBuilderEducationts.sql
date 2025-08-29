CREATE   PROCEDURE [dbo].[AclBuilderAieps]
   @BuilderId int
AS
BEGIN 	
	
	DELETE Acl Where  EntityType='Builder' AND EntityId = @BuilderId 
	
	INSERT INTO Acl (EntityType, EntityId, UserId) 
	SELECT DISTINCT 'Builder', @BuilderId, Educationer.Id
	From  BuilderEducationerAiep bd
	INNER JOIN [User] Educationer on Educationer.AiepId = bd.AiepId
	Where  BuilderId = @BuilderId

END

