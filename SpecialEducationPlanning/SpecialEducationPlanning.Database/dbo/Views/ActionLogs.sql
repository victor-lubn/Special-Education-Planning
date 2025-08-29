CREATE VIEW [dbo].[ActionLogs]
	AS SELECT t.Id
	,t.ActionType
	,t.EntityId
	,t.EntityName
	,t.[User]
	, EntityValue = 
		CASE
			WHEN t.EntityName like '%Builder' THEN t.TradingName
			WHEN t.EntityName like '%Project' THEN t.CodeProject
			WHEN t.EntityName like '%Plan'	  THEN t.PlanCode
			WHEN t.EntityName like '%Version' THEN CAST(t.VersionNumber AS nvarchar(100))
			WHEN t.EntityName like '%Aiep'	  THEN t.AiepCode
			WHEN t.EntityName like '%User'	  THEN t.UserName
			WHEN t.EntityName like '%Comment' THEN t.PlanCommented
		END
	,t.[Date]
FROM (
	SELECT Id
		, ActionType
		, EntityId
		, EntityName
		, [User]
		, [Date]
		, TradingName = (
			SELECT TradingName
			FROM Builder
			WHERE Id = act.EntityId 
			AND act.EntityName like '%Builder'
		)
		, CodeProject = (
			SELECT CodeProject
			FROM Project
			WHERE Id = act.EntityId
			AND act.EntityName like '%Project'
		)
		, PlanCode = (
			SELECT PlanCode
			FROM [Plan]
			WHERE Id = act.EntityId
			AND act.EntityName like '%Plan'
		)
		, VersionNumber = (
			SELECT VersionNumber
			FROM [Version]
			WHERE Id = act.EntityId
			AND act.EntityName like '%Version'
		)
		, AiepCode = (
			SELECT AiepCode
			FROM Aiep
			WHERE Id = act.EntityId
			AND act.EntityName like '%Aiep'
		)
		, UserName = (
			SELECT u.[UniqueIdentifier]
			FROM [User] u
			WHERE Id = act.EntityId
			AND act.EntityName like '%User'
		)
		, PlanCommented = (
			SELECT p.PlanCode
			FROM Comment c
			INNER JOIN [Plan] p on c.EntityId = p.Id
			WHERE c.Id = act.EntityId
			AND act.EntityName like '%Comment'
		)

	FROM [Action] act
) t

