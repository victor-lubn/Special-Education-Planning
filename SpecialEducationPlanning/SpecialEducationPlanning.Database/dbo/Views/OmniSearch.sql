














CREATE VIEW [dbo].[OmniSearch] 
 
AS 
SELECT
      --Plan_Version_Builder_Educationer_EndUser_User_Acl
       [Plan_Id]
      ,[Plan_PlanState]
      ,[Plan_PlanCode]
      ,[Plan_CadFilePlanId]
      ,[Plan_UpdatedDate]
      ,[Plan_Version_Id]
      ,[Plan_Version_ExternalCode]
      ,[Plan_Version_VersionNotes]
      ,[Plan_Builder_Id]
      ,[Plan_Builder_AccountNumber]
      ,[Plan_Builder_TradingName]
      ,[Plan_Builder_Name]
      ,[Plan_Builder_Address0]
      ,[Plan_Builder_Address1]
      ,[Plan_Builder_Postcode]
      ,[Plan_Builder_MobileNumber]
      ,[Plan_Builder_LandLineNumber]
      ,[Plan_Builder_Email]
      ,[Plan_Builder_Notes]
      ,[Plan_EndUser_Id]
      ,[Plan_EndUser_Surname]
      ,[Plan_EndUser_FirstName]
      ,[Plan_EndUser_Address0]
      ,[Plan_EndUser_Address1]
      ,[Plan_EndUser_Postcode]
      ,[Plan_EndUser_MobileNumber]
      ,[Plan_EndUser_LandLineNumber]
      ,[Plan_EndUser_Comment]
      ,[Plan_EndUser_ContactEmail]
      ,[Plan_EndUser_FullName]
      ,[Plan_User_Id]
      ,[Plan_User_UniqueIdentifier]
      ,[Plan_User_Surname]
      ,[Plan_User_FirstName]
      ,[Plan_Acl_UserId]
	  --Builder
	  ,[Builder_Id]
      ,[Builder_UpdatedDate]
      ,[Builder_AccountNumber]
      ,[Builder_TradingName]
      ,[Builder_Name]
      ,[Builder_Address0]
      ,[Builder_Address1]
      ,[Builder_Postcode]
      ,[Builder_MobileNumber]
      ,[Builder_Email]
      ,[Builder_LandLineNumber]
      ,[Builder_Acl_UserId]
	  --Combo
	  ,CONCAT( 'B',[Builder_Id],'P',[Plan_Id] ) AS "Ids" 
	  ,CONCAT( [Builder_UpdatedDate],[Plan_UpdatedDate] ) AS "UpdatedDates" 

FROM
    dbo.[Builder_Acl] bu 
FULL OUTER JOIN dbo.Plan_Version_Builder_Educationer_EndUser_User_Acl pl
    ON 1=0
