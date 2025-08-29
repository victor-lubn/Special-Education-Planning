-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[AclCheckUser]
(
    -- Add the parameters for the stored procedure here
    @userUniqueIdentifier varchar(500)
)
AS
BEGIN



Declare @okValue INT = 0
Declare @nokValue INT = 1



Declare @AiepId int = (SELECT TOP 1 usr.AiepId From [User] usr where UniqueIdentifier = @userUniqueIdentifier)
Declare @UserId int = (SELECT TOP 1   usr.Id From [User] usr where UniqueIdentifier = @userUniqueIdentifier)

PRINT 'User: ' + @userUniqueIdentifier
PRINT 'UserId: ' + STR(@UserId)
PRINT 'AiepId: ' + STR(@AiepId) 
PRINT '------------------------------------'

----Acl Total

Declare @totalUserAcl int = (SELECT count(*) From  Acl a Where  a.UserId = @UserId);

----Builders

Declare @totalBuilderAiep int =  (Select count(*) 
From BuilderEducationerAiep 
Where AiepId = @AiepId)

Declare @totalBuilderAiepAcl int =(Select count(*) As [User BuilderAiepACLs] 
From Acl a 
INNER JOIN BuilderEducationerAiep bd On bd.BuilderId =  a.EntityId
Where bd.AiepId = @AiepId and a.EntityType = 'Builder' and a.UserId = @UserId)

 PRINT 'Builder'    + STR(@totalBuilderAiep)  + STR(@totalBuilderAiepAcl)

IF @totalBuilderAiep <> @totalBuilderAiepAcl 
    BEGIN  
        PRINT 'Builder Error'  
        RETURN @nokValue
	END
ELSE
	BEGIN
		PRINT 'Builder Ok' 		
    END;

PRINT '------------------------------------'
----Plans

Declare @totalPlanAiep int =  
(Select count(*) As [User PlanCount]
From [Plan] pl
INNER JOIN Project pr On pr.Id =  pl.ProjectId
Where pr.AiepId = @AiepId)



Declare @totalPlanAiepAcl int =
(Select count(*) As [User PlanCountACLs] 
From Acl a 
INNER JOIN [Plan] pl On pl.Id = a.EntityId
INNER JOIN Project pr On pr.Id =  pl.ProjectId
Where pr.AiepId = @AiepId and  a.EntityType = 'Plan' and a.UserId = @UserId)

 PRINT 'Plan'    + STR(@totalPlanAiep)  + STR(@totalPlanAiepAcl)

IF @totalPlanAiep <> @totalPlanAiepAcl 
    BEGIN  
        PRINT 'Plan Error'  
        RETURN @nokValue
	END
ELSE
	BEGIN
		PRINT 'Plan Ok' 		
    END;


PRINT '------------------------------------'
----Versions

Declare @totalVersionAiep int =  
(Select count(*) As [User VersionCount]
From [Version] ver
INNER JOIN [Plan] pl On pl.Id = ver.PlanId
INNER JOIN Project pr On pr.Id =  pl.ProjectId
Where pr.AiepId = @AiepId)



Declare @totalVersionAiepAcl int =
(Select count(*)
From Acl a 
INNER JOIN [Version] ver On ver.id = a.EntityId
INNER JOIN [Plan] pl On pl.Id = ver.PlanId
INNER JOIN Project pr On pr.Id =  pl.ProjectId
Where pr.AiepId = @AiepId and  a.EntityType = 'Version' and a.UserId=@UserId)

 PRINT 'Version'    + STR(@totalVersionAiep)  + STR(@totalVersionAiepAcl)

IF @totalVersionAiep <> @totalVersionAiepAcl 
    BEGIN  
        PRINT 'Version Error'  
        RETURN @nokValue
	END
ELSE
	BEGIN
		PRINT 'Version Ok' 		
    END;


PRINT '------------------------------------'
----EndUser

Declare @totalEndUserAiep int =  
(Select count(*) As [User EndUsers]
From EndUser
INNER JOIN [Plan] pl On pl.EndUserId = EndUser.Id
INNER JOIN Project pr On pr.Id =  pl.ProjectId
Where pr.AiepId = @AiepId)



Declare @totalEndUserAiepAcl int =
(Select count(*) As [User EndUsersACLs] 
From Acl a 
INNER JOIN [EndUser] eu on eu.Id = a.EntityId
INNER JOIN [Plan] pl On pl.EndUserId = eu.Id
INNER JOIN Project pr On pr.Id =  pl.ProjectId
Where pr.AiepId = @AiepId and  a.EntityType = 'EndUser' and a.UserId=@UserId)

 PRINT 'EndUser'    + STR(@totalVersionAiep)  + STR(@totalVersionAiepAcl)

IF @totalEndUserAiep <> @totalEndUserAiepAcl 
    BEGIN  
        PRINT 'EndUser Error'  
        RETURN @nokValue
	END
ELSE
	BEGIN
		PRINT 'EndUser Ok' 		
    END;


return @okValue











       
      














END

