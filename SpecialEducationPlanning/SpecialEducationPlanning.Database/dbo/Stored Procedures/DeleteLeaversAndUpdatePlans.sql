/****** Object:  StoredProcedure [dbo].[DeleteLeaversAndUpdatePlans]    Script Date: 10/08/2021 13:34:16 ******/

CREATE PROCEDURE [dbo].[DeleteLeaversAndUpdatePlans]
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.

		SET NOCOUNT OFF
		DECLARE @ErrorMessage NVARCHAR(4000)  
		DECLARE @ErrorSeverity INT 
		DECLARE @ErrorState INT 
		DECLARE @LogLevel NVARCHAR(20)
		SET @LogLevel='Information'

	--Existing ex Educationers
		IF OBJECT_ID('tempdb..#TEMP_UsersToBeDeleted') IS NOT NULL 
		BEGIN 
			DROP TABLE #TEMP_UsersToBeDeleted 
		END	

		SELECT  Id, AiepId, Leaver, DelegateToUserId,[UniqueIdentifier], ROW_NUMBER() OVER (ORDER BY Id ASC) AS Rownumber
		INTO #TEMP_UsersToBeDeleted
		FROM [User] 
		WHERE ([UniqueIdentifier] LIKE '%delete%' OR Firstname LIKE '%delete%' OR surname  LIKE '%delete%'
		OR [UniqueIdentifier] LIKE '%deletion%' OR Firstname LIKE '%deletion%' OR surname  LIKE '%deletion%'
		OR [UniqueIdentifier] LIKE '%remove%' OR Firstname LIKE '%remove%' OR surname  LIKE '%remove%')
		AND Leaver<>1 		

		DECLARE @userToBeDeletedCount INT		
		DECLARE @AiepId INT=0
		DECLARE @EducationerId INT=0
		DECLARE @UPN VARCHAR(450)
		DECLARE @PlanUpdateUserId INT=0
		DECLARE @AiepManagerId INT=0
		DECLARE @LeaversRemaining INT		
		DECLARE @DeletedRows INT=0	
		DECLARE @ManagerCount INT=0
		DECLARE @PlanByExUserCount INT =0
		DECLARE @ErrorCount INT =0

		--select * from #TEMP_UsersToBeDeleted

		SET @userToBeDeletedCount=(SELECT COUNT(*) FROM #TEMP_UsersToBeDeleted)
		IF(@userToBeDeletedCount>0)
		BEGIN
			DECLARE @UsersCounter INT=1
			WHILE(@UsersCounter<=@userToBeDeletedCount)			
			BEGIN
				BEGIN TRY
					BEGIN TRANSACTION
						UPDATE [USER]SET Leaver=1, DelegateToUserId=0 WHERE Id IN (SELECT Id FROM #TEMP_UsersToBeDeleted)
					COMMIT TRANSACTION
					SET @UsersCounter=@UsersCounter+1
				END TRY
				BEGIN CATCH
					ROLLBACK TRANSACTION
				END CATCH
			END
		END

			IF OBJECT_ID('tempdb..#TEMP_LeaverDetails') IS NOT NULL 
			BEGIN 
				DROP TABLE #TEMP_LeaverDetails 
			END	

			SET @LogLevel=''
			SELECT id,Leaver,DelegateToUserId,[UniqueIdentifier],ROW_NUMBER() OVER (ORDER BY Id ASC) AS Rownumber,AiepId
			INTO #TEMP_LeaverDetails
			FROM [USER]
			WHERE Leaver=1
		
			DECLARE @leaversCount INT
			DECLARE @SelectedUserEmail VARCHAR(400)

			SET @leaversCount=(SELECT COUNT(*) FROM #TEMP_LeaverDetails)
			IF(@leaversCount>0)
			BEGIN					
				DECLARE @Counter INT=1
				DECLARE @DelegateUserId INT=0
				DECLARE @SelectedLeaverAiepId INT
				SET @ErrorCount=0
				WHILE(@Counter<=@leaversCount)
				BEGIN
					IF OBJECT_ID('tempdb..#TEMP_SelectedLeaver') IS NOT NULL 
					BEGIN 
						DROP TABLE #TEMP_SelectedLeaver 
					END						

					SELECT * 
					INTO #TEMP_SelectedLeaver
					FROM #TEMP_LeaverDetails 
					WHERE Rownumber=@Counter
					
					--SELECT *from #TEMP_SelectedLeaver

					DECLARE @LeaverId INT	
					DECLARE @PlanCountRemaining INT
					DECLARE @AiepsMovedByLeaverCount INT					
		
					SELECT @LeaverId=Id,@DelegateUserId=DelegateToUserId,@SelectedUserEmail=[UniqueIdentifier],@SelectedLeaverAiepId=[AiepId]
					FROM #TEMP_SelectedLeaver WHERE Rownumber=@Counter
		
					--select @leaversCount	
					BEGIN TRY
					BEGIN TRANSACTION
							
							IF OBJECT_ID('tempdb..#TempLeaversCurrentAiepPlans') IS NOT NULL 
							BEGIN 
								DROP TABLE #TempLeaversCurrentAiepPlans 
							END	
							SELECT *
							INTO #TempLeaversCurrentAiepPlans
							FROM [PLAN]
							WHERE EducationerId=@LeaverId AND ProjectId IN(SELECT Id FROM Project WHERE AiepId=@SelectedLeaverAiepId)
							AND EducationerId NOT IN (SELECT Id FROM [USER] WHERE DelegateToUserId=0)

							--SELECT * FROM #TempLeaversCurrentAiepPlans
					
							UPDATE [PLAN] SET EducationerId=@DelegateUserId
							--SELECT * FROM [PLAN]
							WHERE Id IN(SELECT Id FROM #TempLeaversCurrentAiepPlans)
							AND @DelegateUserId<>0
							
							SET @PlanCountRemaining =(SELECT COUNT(*) FROM [Plan] WHERE EducationerId=@LeaverId)
							IF(@PlanCountRemaining>0)
							BEGIN
								IF OBJECT_ID('tempdb..#TempRestOfThePlans') IS NOT NULL 
								BEGIN 
									DROP TABLE #TempRestOfThePlans 
								END	

								SELECT p.Id,EducationerId,pr.AiepId,P.UpdateUser,p.CreationUser
								INTO #TempRestOfThePlans
								FROM [Plan] P JOIN Project Pr ON P.ProjectId=Pr.Id
								WHERE EducationerId=@LeaverId
								AND P.Id NOT IN (SELECT Id from #TempLeaversCurrentAiepPlans)

								--SELECT * FROM #TempRestOfThePlans

								IF OBJECT_ID('tempdb..#TempAiepsMovedByLeaver') IS NOT NULL 
								BEGIN 
									DROP TABLE #TempAiepsMovedByLeaver 
								END	
								SELECT DISTINCT AiepId 
								INTO #TempAiepsMovedByLeaver
								FROM #TempRestOfThePlans

								SET @AiepsMovedByLeaverCount= (SELECT COUNT(AiepId) FROM #TempAiepsMovedByLeaver )

								IF(@AiepsMovedByLeaverCount>=1)
								BEGIN
									IF OBJECT_ID('tempdb..#TempAiepsMoved') IS NOT NULL 
									BEGIN 
										DROP TABLE #TempAiepsMoved 
									END	
									SELECT AiepId,Row_Number() OVER (ORDER BY  AiepId DESC) AS RowNumber
									INTO #TempAiepsMoved
									FROM #TempAiepsMovedByLeaver
									 

									--SELECT * FROM #TempAiepsMoved
									
									DECLARE @PlanCounter INT=1
									DECLARE @AiepIdMoved INT
									DECLARE @PreviousAiepManagerId INT
									DECLARE @AssigneeEducationerId INT 

									WHILE(@PlanCounter<=@AiepsMovedByLeaverCount)
									BEGIN
										SET @AiepIdMoved=(SELECT AiepId FROM #TempAiepsMoved WHERE RowNumber=@PlanCounter)

										--SELECT @AiepIdMoved
									
										SET @PreviousAiepManagerId=
										(SELECT Top(1)U.Id FROM
										[USER] U 
										JOIN [UserRole] UR ON U.Id=UR.UserId
										JOIN [Role] R ON R.Id=UR.RoleId
										WHERE R.[Name]='AiepManager' AND U.AiepId=@AiepIdMoved
										and U.Leaver=0
										ORDER BY U.Id DESC)

										--select @PreviousAiepManagerId as PreviousAiepManagerId

										SET @AssigneeEducationerId=(SELECT Top(1)U.Id FROM
										[USER] U 
										JOIN [UserRole] UR ON U.Id=UR.UserId
										JOIN [Role] R ON R.Id=UR.RoleId
										WHERE R.[Name]='Educationer' AND U.AiepId=@AiepIdMoved
										and U.Leaver=0
										ORDER BY U.Id DESC)

										--select @AssigneeEducationerId as AssigneeEducationerId	
										
										IF OBJECT_ID('tempdb..#tempPlanAccessed') IS NOT NULL 
										BEGIN 
											DROP TABLE #tempPlanAccessed 
										END	
										
										SELECT p.Id,a.CreationUser,b.UpdateUser,u.Id as 'UpdateUserId'
										INTO #tempPlanAccessed
										FROM [Plan] P 
										JOIN [Version] a ON p.Id = a.PlanId	AND a.id =(SELECT TOP(1) Id FROM [Version]v WHERE v.PlanId=P.Id ORDER BY v.UpdatedDate desc)						
										JOIN [Version] b ON p.Id=b.PlanId AND b.id =(SELECT TOP(1) Id FROM [Version]v WHERE v.PlanId=P.Id ORDER BY v.UpdatedDate desc) and a.Id=b.Id
										JOIN [User]U ON b.UpdateUser=u.[UniqueIdentifier]
										WHERE p.Id IN (SELECT Id FROM  #TempRestOfThePlans WHERE AiepId =@AiepIdMoved)										
										AND b.UpdateUser IS NOT NULL
										AND p.CreationUser<>b.UpdateUser
										--AND U.Leaver=0
										AND b.UpdateUser NOT IN (SELECT [UniqueIdentifier] FROM [USER] WHERE Leaver=1 AND AiepId=@AiepIdMoved)
										AND b.UpdateUser IN(SELECT [UniqueIdentifier] FROM [User] WHERE AiepId=@AiepIdMoved )


										--SELECT * from #tempPlanAccessed

										IF((SELECT COUNT(*) FROM #tempPlanAccessed)>0)
										BEGIN
											UPDATE p SET p.EducationerId=pa.UpdateUserId
											--SELECT p.id,p.EducationerId,pa.UpdateUserId,PA.UpdateUser
											FROM [Plan]p, #tempPlanAccessed pa 
											WHERE p.Id = pa.Id
										END

										IF((SELECT COUNT(*) FROM Aiep WHERE ManagerId=@LeaverId)>0 )
										BEGIN
											IF(@PreviousAiepManagerId IS NOT NULL )
											BEGIN
												UPDATE Aiep SET ManagerId=@PreviousAiepManagerId WHERE ManagerId=@LeaverId
											END
											ELSE
											IF(@AssigneeEducationerId IS NOT NULL)
											BEGIN
												UPDATE Aiep SET ManagerId=@AssigneeEducationerId WHERE ManagerId=@LeaverId
											END
											ELSE
											BEGIN
												UPDATE Aiep SET ManagerId=NULL WHERE ManagerId=@LeaverId
											END
										END

										IF (@PreviousAiepManagerId IS NOT NULL)
										BEGIN
											UPDATE P SET EducationerId=@PreviousAiepManagerId
											--SELECT EducationerId,@PreviousAiepManagerId,AiepId 
											FROM [Plan]P
											JOIN [PROJECT] PR ON P.ProjectId=PR.Id
											WHERE P.id IN
											(SELECT Id FROM #TempRestOfThePlans 
											 WHERE AiepId=@AiepIdMoved 
											)
											AND P.id NOT IN (SELECT Id FROM #tempPlanAccessed )
											AND PR.AiepId=@AiepIdMoved
										END
										ELSE
										BEGIN
											UPDATE P SET EducationerId=@AssigneeEducationerId
											--SELECT  EducationerId,@AssigneeEducationerId,AiepId 
											FROM [Plan]P
											JOIN [PROJECT] PR ON P.ProjectId=PR.Id
											WHERE P.id IN
											(SELECT Id FROM #TempRestOfThePlans
											WHERE AiepId=@AiepIdMoved)
											AND P.id NOT IN (SELECT Id FROM #tempPlanAccessed )
											AND PR.AiepId=@AiepIdMoved
										END

										SET @PlanCounter=@PlanCounter+1
									END								

								END
							END								
							
								DELETE UserReleaseInfo
								WHERE userId=@LeaverId
							
								DELETE [UserRole]  
								WHERE UserId=@LeaverId
							
								DELETE ACL 
								WHERE EntityType='User' AND EntityId=@LeaverId
							
								DELETE [USER]
								WHERE ID=@LeaverId AND DelegateToUserId IS NOT NULL
							
								EXEC AclEducationer @DelegateUserId
								COMMIT TRANSACTION																		
							
					END TRY
					BEGIN CATCH	
						ROLLBACK TRANSACTION
						SET @ErrorCount=@ErrorCount+1
						SELECT @ErrorMessage = ERROR_MESSAGE(),@ErrorState = ERROR_STATE();  
						SET @LogLevel='Error'
						-- Use RAISERROR inside the CATCH block to return error  
						-- information about the original error that caused  
						-- execution to jump to the CATCH block.  
						RAISERROR (@ErrorMessage, -- Message text.  
									@ErrorSeverity, -- Severity.  
									@ErrorState -- State.  
									)
									SELECT @ErrorMessage
						INSERT INTO [Log] ([Message],[MessageTemplate],[Level],[TimeStamp],[Exception])
						(SELECT 'Error deleting user ',CAST(@SelectedUserEmail AS VARCHAR(100))+ ' could not be deleted','Error',GETDATE(),@ErrorMessage)
				END CATCH
				SET @Counter=@Counter+1	
			END
				SET @DeletedRows=(SELECT ((@Counter-1)-@ErrorCount))
				INSERT INTO [Log] ([Message],[MessageTemplate],[Level],[TimeStamp],[Exception])
				(SELECT 'Users Deleted',CAST(@DeletedRows AS VARCHAR(100))+' rows deleted','Information',GETDATE(),NULL)				
			END

			SET @LeaversRemaining= (SELECT COUNT([UniqueIdentifier]) as 'Remaining Ex-Educationers' FROM [USER]
			WHERE([UniqueIdentifier] LIKE '%delete%' OR Firstname LIKE '%delete%' OR surname  LIKE '%delete%'
				OR [UniqueIdentifier] LIKE '%deletion%' OR Firstname LIKE '%deletion%' OR surname  LIKE '%deletion%'
				OR [UniqueIdentifier] LIKE '%remove%' OR Firstname LIKE '%remove%' OR surname  LIKE '%remove%')
				OR Leaver=1)
			
			INSERT INTO DeleteLeaversJobReport(NumberOfLeaversDeleted,NumberOfRemainingLeavers)
			(SELECT @DeletedRows,@LeaversRemaining)

			--DROP TABLE #TEMP_LeaverDetails
END

