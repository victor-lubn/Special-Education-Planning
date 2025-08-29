CREATE   PROCEDURE [dbo].[AclProjectAiep]
   @ProjectId int  
AS
BEGIN 

	Delete a From Acl a	
	Where  a.EntityId = @ProjectId and a.EntityType = 'Project'
		
	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Project', p.Id, Educationer.Id
	From  Project p
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  p.Id = @ProjectId

	Delete a From Acl a	
	INNER JOIN [Plan] pl on pl.Id =  a.EntityId
	INNER JOIN [Project] p on pl.ProjectId =  p.Id
	Where a.EntityType = 'Plan' and p.Id= @ProjectId

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Plan', pl.Id, Educationer.Id
	From  [Plan] pl
	INNER JOIN [Project] p on p.Id =  pl.ProjectId
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  pl.ProjectId =  @ProjectId


	Delete a From Acl a	
	INNER JOIN [EndUser] eu on eu.Id =  a.EntityId
	INNER JOIN [Plan] pl on pl.EndUserId =  eu.Id
	INNER JOIN [Project] p on pl.ProjectId =  p.Id
	Where a.EntityType = 'EndUser' and p.Id= @ProjectId
	and a.UserId IN (
		SELECT u.Id
		FROM [User] u 
		WHERE u.AiepId = p.AiepId)

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'EndUser', eu.Id, Educationer.Id
	From  [EndUser] eu
	INNER JOIN [Plan] pl on pl.EndUserId =  eu.Id
	INNER JOIN [Project] p on p.Id =  pl.ProjectId	
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  pl.ProjectId =  @ProjectId  and Educationer.AiepId = p.AiepId

	Delete a From Acl a	
	INNER JOIN [Version] v on v.Id =  a.EntityId
	INNER JOIN [Plan] pl on pl.Id =  v.PlanId
	INNER JOIN [Project] p on pl.ProjectId =  p.Id
	Where a.EntityType = 'Version' and p.Id= @ProjectId

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Version',v.Id, Educationer.Id
	From  [Version] v
	INNER JOIN [Plan] pl on pl.Id =  v.PlanId
	INNER JOIN [Project] p on p.Id =  pl.ProjectId
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  pl.ProjectId =  @ProjectId



END

