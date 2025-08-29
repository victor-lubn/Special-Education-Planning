

CREATE PROCEDURE [dbo].[sp_RemoveDuplicateSAPAccounts] 
AS
BEGIN
		
	DECLARE @plansUpdated INT=0
	DECLARE @BuilderEducationerAiep INT=0
	DECLARE @ACLCreated INT=0
	DECLARE @BuildersDeleted INT=0
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT; 
	DECLARE @DuplicateAccountsCount INT=0;
	DECLARE @Result varchar(200)='Plans updated: 0  BuilderEducationerAieps created: 0  ACL created: 0  Builders deleted: 0'

	   --Get all the Duplicate SAP Accounts
		IF OBJECT_ID('tempdb..#temp_DupBuilders') IS NOT NULL
		BEGIN
			DROP TABLE #temp_DupBuilders
		END
		CREATE TABLE #temp_DupBuilders
		(Accountnumber VARCHAR(30),
		Counts INT)

		INSERT INTO #temp_DupBuilders(Accountnumber,Counts)  
		(SELECT accountNumber,COUNT(accountnumber) AS counts FROM Builder 
		GROUP BY accountnumber 
		HAVING COUNT(accountnumber)>1 AND accountnumber IS NOT NULL  
		and RTRIM(LTRIM(accountnumber))<>'' and ISNUMERIC(accountnumber)=1 )

		SET @DuplicateAccountsCount=(SELECT COUNT(*) FROM #temp_DupBuilders)

	--If there are any duplicate SAP accounts
	IF (@DuplicateAccountsCount>0)
	BEGIN
	
		--Get latest of duplicate accounts with same account number
		IF OBJECT_ID('tempdb..#temp_LatestBuilderAccount') IS NOT NULL
		BEGIN
			DROP TABLE #temp_LatestBuilderAccount
		END
		SELECT MAX(ID) AS LatestId, AccountNumber 
		INTO #temp_LatestBuilderAccount
		FROM BUILDER
		GROUP BY AccountNumber
		HAVING AccountNumber IN 
		(SELECT AccountNumber FROM #temp_DupBuilders)

		--Get Other accounts than latest for same account number
		IF OBJECT_ID('tempdb..#temp_OldBuilderAccount') IS NOT NULL
		BEGIN
			DROP TABLE #temp_OldBuilderAccount
		END
		SELECT ID AS OldBuilderId, AccountNumber
		INTO #temp_OldBuilderAccount
		FROM Builder
		WHERE AccountNumber IN (SELECT AccountNumber FROM #temp_DupBuilders) 
		AND ID NOT IN(SELECT LatestId FROM #temp_LatestBuilderAccount)

		--Create pivoted table merging both old and latest accounts
		IF OBJECT_ID('tempdb..#temp_allBuilders') IS NOT NULL
		BEGIN
			DROP TABLE #temp_allBuilders
		END
		SELECT L.AccountNumber,L.LatestId,OldBuilderId,T.TradingName,
		ROW_NUMBER() OVER (ORDER BY L.AccountNumber ASC) AS Rownumber
		INTO #temp_allBuilders
		FROM #temp_LatestBuilderAccount L JOIN 
		#temp_OldBuilderAccount O ON L.AccountNumber=O.AccountNumber
		JOIN (SELECT TradingName,LatestId from Builder B 
		JOIN #temp_LatestBuilderAccount L ON b.Id=l.LatestId)T ON T.LatestId=L.LatestId

		DECLARE @numberOfDuplicateAccounts INT
		DECLARE @i INT
		SET @numberOfDuplicateAccounts= (SELECT COUNT(*) FROM #temp_allBuilders)
		SET @i=1	

		WHILE(@i<=@numberOfDuplicateAccounts)
		BEGIN
			BEGIN TRY
				--Traverse through each record and get details 
				IF OBJECT_ID('tempdb..#temp_ToBeUpdatedWith') IS NOT NULL
				BEGIN
					DROP TABLE #temp_ToBeUpdatedWith
				END
				SELECT A.AccountNumber, LatestId,OldBuilderId, TradingName
				INTO #temp_ToBeUpdatedWith
				FROM #temp_allBuilders A 
				WHERE Rownumber=@i
			
				DECLARE @OldBuilderId INT
				SET @OldBuilderId=(SELECT OldBuilderId FROM #temp_ToBeUpdatedWith)
				DECLARE @LatestId INT
				SET @LatestId=(SELECT LatestId FROM #temp_ToBeUpdatedWith)
						 
				--Update plans with latest builder Id and Trading Name
				UPDATE [PLAN] SET BuilderId =U.LatestId, BuilderTradingName=U.TradingName,
				UpdateUser='Duplicate Accounts CleanUp',UpdatedDate=GETDATE()
				FROM
				(SELECT * FROM #temp_ToBeUpdatedWith)U
				WHERE BuilderId=OldBuilderId
				SET @plansUpdated= @plansUpdated+@@ROWCOUNT 	

				IF OBJECT_ID('tempdb..#temp_AllBuilderEducationerAiep') IS NOT NULL
				BEGIN
					DROP TABLE #temp_AllBuilderEducationerAiep
				END
				SELECT DISTINCT *,ROW_NUMBER() OVER (ORDER BY AiepId ASC) AS Rownumber 
				INTO #temp_AllBuilderEducationerAiep
				FROM (SELECT DISTINCT * FROM BuilderEducationerAiep WHERE BuilderId in (@OldBuilderId,@LatestId))BD

				--Delete old BuilderEducationerAiep entry and insert olbuilders Aiep with new builder
				----BuilderEducationerAiep BEGIN				

				DELETE BuilderEducationerAiep WHERE BuilderId IN (@OldBuilderId,@LatestId)

				INSERT INTO BuilderEducationerAiep(BuilderId, AiepId) 
				SELECT DISTINCT  @LatestId, D.AiepId
				From
				(SELECT * FROM #temp_AllBuilderEducationerAiep)D
			
				SET @BuilderEducationerAiep=(SELECT COUNT(*)FROM BuilderEducationerAiep WHERE BuilderId=@LatestId)
				--BuilderEducationerAiep END

				--ACL BEGIN

				DELETE Acl WHERE entityId=@OldBuilderId and EntityType='Builder'

				DECLARE @j INT=1
				DECLARE @AiepCount INT
				SET @AiepCount=(SELECT COUNT(*) FROM #temp_AllBuilderEducationerAiep)

				WHILE(@AiepCount>0 AND @j<=@AiepCount)
				BEGIN
					DECLARE @AiepId INT
					SET @AiepId= (SELECT AiepId FROM #temp_AllBuilderEducationerAiep WHERE Rownumber=@j)					
					EXEC  [dbo].[AclAiep] @AiepId 

					SET @j=@j+1
				END

				IF(@AiepCount=0)
				BEGIN
					IF OBJECT_ID('tempdb..#temp_OldBuilderEducationerAieps') IS NOT NULL
					BEGIN
						DROP TABLE #temp_OldBuilderEducationerAieps
					END
					SELECT DISTINCT AiepId,ROW_NUMBER() OVER (ORDER BY AiepId ASC) AS Rownumber
					INTO #temp_OldBuilderEducationerAieps
					FROM [User] WHERE Id IN(SELECT DISTINCT EducationerId 					
					FROM [Plan] WHERE BuilderId=@OldBuilderId)

					DECLARE @k INT=1
					DECLARE @EducationerAiepCount INT
					SET @EducationerAiepCount=(SELECT COUNT(*) FROM #temp_OldBuilderEducationerAieps)

					WHILE(@EducationerAiepCount>0 AND @k<=@EducationerAiepCount)
					BEGIN
						DECLARE @EducationerAiepId INT
						SET @EducationerAiepId= (SELECT AiepId FROM #temp_OldBuilderEducationerAieps WHERE Rownumber=@k)					
						EXEC  [dbo].[AclAiep] @EducationerAiepId 

						SET @k=@k+1
					END
                    DROP TABLE #temp_OldBuilderEducationerAieps
				END
			
				--SET @ACLCreated=@ACLCreated+@@ROWCOUNT
				SET @ACLCreated=(SELECT COUNT(*)FROM Acl WHERE EntityId=@LatestId AND EntityType='Builder')
							 
				----ACL END				

				--Remove older Builder accounts
				DELETE BUILDER WHERE Id =@OldBuilderId
				SET @BuildersDeleted= (SELECT COUNT(*) FROM #temp_allBuilders WHERE LatestId=@LatestId)

				SET @i=@i+1
			END TRY 
			BEGIN CATCH		
				DECLARE @CurrentTime datetime=(SELECT GETDATE())	

				SELECT @ErrorMessage = ERROR_MESSAGE(),@ErrorState = ERROR_STATE();  
				 
				-- Use RAISERROR inside the CATCH block to return error  
				-- information about the original error that caused  
				-- execution to jump to the CATCH block.  
				RAISERROR (@ErrorMessage, -- Message text.  
						   @ErrorSeverity, -- Severity.  
						   @ErrorState -- State.  
						   );  
			END CATCH;
            
		SET @Result=('Plans updated: '+CONVERT(VARCHAR(5),@plansUpdated)+
		'  BuilderEducationerAieps created: '+CONVERT(VARCHAR(5),@BuilderEducationerAiep)+
		'  ACL created: '+CONVERT(VARCHAR(5),@ACLCreated)+
		'  Builders deleted: '+CONVERT(VARCHAR(5),@BuildersDeleted )) 

		DECLARE @DupOldBuilderIds VARCHAR(MAX)
		SET @DupOldBuilderIds= (SELECT DISTINCT	STUFF((
		SELECT ', ' + CAST(d.OldBuilderId AS VARCHAR(200)) FROM (
		SELECT OldBuilderId,LatestId FROM #temp_allBuilders 		
		GROUP BY LatestId,OldBuilderId HAVING LatestId=@LatestId)D FOR XML PATH('')), 1, 2, '') 
		FROM #temp_allBuilders )

		IF ((SELECT COUNT(*) FROM DuplicateBuildersLog WHERE NewBuilderId=@LatestId)=0)
		BEGIN
			INSERT INTO DuplicateBuildersLog(OldBuilderId,NewBuilderId,Result,CreatedDate,ErrorMessage,ErrorState)
			SELECT @DupOldBuilderIds,@LatestId,@Result,Getdate(),@ErrorMessage,@ErrorState		

			SET @plansUpdated=0
			SET @BuilderEducationerAiep=0
			SET @BuildersDeleted=0
			SET @ACLCreated=0
			SET @ErrorMessage=NULL
			SET @ErrorState=NULL
			SET @DupOldBuilderIds=''

		END
		
        END	

		DROP TABLE #temp_OldBuilderAccount
		DROP TABLE #temp_DupBuilders
		DROP TABLE #temp_LatestBuilderAccount		
		DROP TABLE #temp_allBuilders
		DROP TABLE #temp_ToBeUpdatedWith
		DROP TABLE #temp_AllBuilderEducationerAiep	

	END

		--IF(@DuplicateAccountsCount=0)
		--BEGIN
		--	INSERT INTO DuplicateBuildersLog(OldBuilderId,NewBuilderId,Result,CreatedDate,ErrorMessage,ErrorState)
		--	SELECT NULL,NULL,@Result,Getdate(),@ErrorMessage,@ErrorState
		--END

	-- SELECT ISNULL((SELECT COUNT(accountnumber)  
	-- FROM Builder 
	-- GROUP BY accountnumber 
	-- HAVING COUNT(accountnumber)>1 AND accountnumber IS NOT NULL 
	-- AND RTRIM(LTRIM(accountnumber))<>'' AND ISNUMERIC(accountnumber)=1),0)AS 'Duplicate SAP Accounts'

END

