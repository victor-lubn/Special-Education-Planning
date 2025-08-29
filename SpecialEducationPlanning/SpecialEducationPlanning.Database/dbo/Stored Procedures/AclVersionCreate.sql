CREATE   PROCEDURE [dbo].[AclVersionCreate]
   @VersionId int  
AS
BEGIN 

	INSERT INTO Acl (EntityType, EntityId, UserId)
    SELECT DISTINCT 'Version', @VersionId, Educationer.Id
    From  [Version] v
    INNER JOIN [Plan] pl on pl.Id =  v.PlanId
    INNER JOIN [Project] p on p.Id =  pl.ProjectId
    INNER JOIN [User] Educationer on Educationer.AiepId =  p.AiepId
    Where  v.Id =  @VersionId



END

