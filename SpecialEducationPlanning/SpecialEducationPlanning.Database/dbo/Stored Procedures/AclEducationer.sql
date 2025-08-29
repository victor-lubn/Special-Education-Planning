
CREATE PROCEDURE [dbo].[AclEducationer]
   @EducationerId int  
AS
BEGIN 
	
	Delete a From Acl a	
	Where  a.UserId = @EducationerId

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT 'Aiep', AiepId, Id
	From  [User]
	Where  Id =  @EducationerId	
			
	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Project', p.Id, Educationer.Id
	From  Project p
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  Educationer.Id = @EducationerId 

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Plan', pl.Id, Educationer.Id
	From  [Plan] pl
	INNER JOIN [Project] p on p.Id =  pl.ProjectId
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  Educationer.Id = @EducationerId

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'EndUser', eu.Id, Educationer.Id
	From  [EndUser] eu
	INNER JOIN [Plan] pl on pl.EndUserId =  eu.Id
	INNER JOIN [Project] p on p.Id =  pl.ProjectId	
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  Educationer.Id = @EducationerId

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Version',v.Id, Educationer.Id
	From  [Version] v
	INNER JOIN [Plan] pl on pl.Id =  v.PlanId
	INNER JOIN [Project] p on p.Id =  pl.ProjectId
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  Educationer.Id = @EducationerId

	INSERT INTO Acl (EntityType, EntityId, UserId) 
	SELECT DISTINCT 'Builder', bd.BuilderId, Educationer.Id
	From  BuilderEducationerAiep bd
	INNER JOIN [User] Educationer on Educationer.AiepId =  bd.AiepId
	Where  Educationer.Id = @EducationerId

END

