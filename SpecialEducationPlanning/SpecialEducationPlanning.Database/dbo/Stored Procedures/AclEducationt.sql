
-- **
-- Not Filter because entities are related with one Aiep only
-- Performance

CREATE PROCEDURE [dbo].[AclAiep]
   @AiepId int  
AS
BEGIN 
	
	--Aiep

	Delete a From Acl a	
	Where  a.EntityId = @AiepId and a.EntityType = 'Aiep'
	-- Not filtered **

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Aiep', de.Id, Educationer.Id
	From  [Aiep] de	
	INNER JOIN [User] Educationer on Educationer.AiepId = de.Id
	Where  de.Id =  @AiepId and Educationer.AiepId = @AiepId	
	
	--Project

	Delete a From Acl a		
	INNER JOIN [Project] p on p.Id = a.EntityId
	INNER JOIN [Aiep] de on de.Id = p.AiepId	
	Where  a.EntityType = 'Project' and p.AiepId = @AiepId
	-- Not filtered ** 
	--and a.UserId IN (
	--	SELECT u.Id
	--	FROM [User] u 
	--	WHERE u.AiepId = @AiepId)
		
	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Project', p.Id, Educationer.Id
	From  Project p
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  p.AiepId =@AiepId and Educationer.AiepId = @AiepId 

	--Plan

	Delete a From Acl a	
	INNER JOIN [Plan] pl on pl.Id =  a.EntityId
	INNER JOIN [Project] p on pl.ProjectId =  p.Id
	Where a.EntityType = 'Plan' and p.AiepId= @AiepId 
	-- Not filtered **
	--and a.UserId IN (
	--	SELECT u.Id
	--	FROM [User] u 
	--	WHERE u.AiepId = @AiepId)

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Plan', pl.Id, Educationer.Id
	From  [Plan] pl
	INNER JOIN [Project] p on p.Id =  pl.ProjectId
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  p.AiepId = @AiepId and Educationer.AiepId = @AiepId 

	--Version

	Delete a From Acl a	
	INNER JOIN [Version] v on v.Id =  a.EntityId
	INNER JOIN [Plan] pl on pl.Id =  v.PlanId
	INNER JOIN [Project] p on pl.ProjectId =  p.Id
	Where a.EntityType = 'Version' and p.AiepId= @AiepId 
	-- Not filtered **
	--and a.UserId IN (
	--	SELECT u.Id
	--	FROM [User] u 
	--	WHERE u.AiepId = @AiepId)

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Version',v.Id, Educationer.Id
	From  [Version] v
	INNER JOIN [Plan] pl on pl.Id =  v.PlanId
	INNER JOIN [Project] p on p.Id =  pl.ProjectId
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  p.AiepId =  @AiepId and Educationer.AiepId = @AiepId
	
	--EndUser

	Delete a From Acl a	
	INNER JOIN [EndUser] eu on eu.Id =  a.EntityId
	INNER JOIN [Plan] pl on pl.EndUserId =  eu.Id
	INNER JOIN [Project] p on pl.ProjectId =  p.Id
	Where a.EntityType = 'EndUser' and p.AiepId= @AiepId 
	and a.UserId IN (
		SELECT u.Id
		FROM [User] u 
		WHERE u.AiepId = @AiepId)

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'EndUser', eu.Id, Educationer.Id
	From  [EndUser] eu
	INNER JOIN [Plan] pl on pl.EndUserId =  eu.Id
	INNER JOIN [Project] p on p.Id =  pl.ProjectId	
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  p.AiepId =  @AiepId and Educationer.AiepId = @AiepId

	--Builder

	Delete a From Acl a
	INNER JOIN BuilderEducationerAiep bd on bd.BuilderId =  a.EntityId	
	Where  bd.AiepId = @AiepId and a.EntityType = 'Builder' 
	and a.UserId IN (
		SELECT u.Id
		FROM [User] u 
		WHERE u.AiepId = @AiepId)
	
	INSERT INTO Acl (EntityType, EntityId, UserId) 
	SELECT DISTINCT 'Builder', bd.BuilderId, Educationer.Id
	From  BuilderEducationerAiep bd
	INNER JOIN [User] Educationer on Educationer.AiepId =  bd.AiepId
	Where  Educationer.AiepId =  @AiepId and bd.AiepId = @AiepId

END

