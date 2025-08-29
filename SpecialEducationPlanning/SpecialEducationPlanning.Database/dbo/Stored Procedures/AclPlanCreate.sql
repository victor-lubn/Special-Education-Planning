CREATE   PROCEDURE [dbo].[AclPlanCreate]
   @PlanId int  
AS
BEGIN 

	INSERT INTO Acl (EntityType, EntityId, UserId)
	SELECT DISTINCT 'Plan', @PlanId, Educationer.Id
	From  [Plan] pl
	INNER JOIN [Project] p on p.Id =  pl.ProjectId
	INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
	Where  pl.Id =  @PlanId



END

