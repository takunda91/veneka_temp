-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Approve's the load batch. if issuer has auto create dist batch then create distbatch
-- =============================================
CREATE PROCEDURE [dbo].[usp_load_batch_approve] 
	@load_batch_id bigint,
	@status_notes varchar(150),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	BEGIN TRANSACTION [APPROVE_LOAD_BATCH]
		BEGIN TRY 
			
			DECLARE @current_load_batch_status_id int,
					@issuer_id int,
					@dist_batch_id bigint,
					@number_of_dist_cards int,
					@dist_batch_ref varchar(50),
					@dist_batch_status_name varchar(50),
					@audit_msg varchar(max)

			SELECT @current_load_batch_status_id = load_batch_statuses_id
			FROM load_batch_status_current
			WHERE load_batch_id = @load_batch_id
										  
			--Check that someone hasn't already updated the load batch
			IF(@current_load_batch_status_id != 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN

					DECLARE @product_load_type_id int,
							@auto_approve bit,
							@card_issue_method_id int
										
					--STEP ONE: What do we do with the product in the load batch?
					--ASSUMPTION IS THAT LOAD IS DONE PER PRODUCT
					IF( (SELECT COUNT(DISTINCT product_id)
						 FROM [cards] INNER JOIN [load_batch_cards]
							ON [load_batch_cards].card_id = [cards].card_id
						 WHERE [load_batch_cards].load_batch_id = @load_batch_id) > 1)
						RAISERROR ('Multiple products found in batch.',	12,	12);

					--Get the load type for the product
					SELECT @product_load_type_id = product_load_type_id,
							@auto_approve = auto_approve_batch_YN,
							@card_issue_method_id = card_issue_method_id
					FROM [issuer_product] 
					WHERE product_id = (SELECT TOP 1 product_id 
										FROM [cards] INNER JOIN [load_batch_cards]
											ON [load_batch_cards].card_id = [cards].card_id
										WHERE [load_batch_cards].load_batch_id = @load_batch_id)

					--STEP TWO-a: Create Production Batch
					IF(@product_load_type_id = 1 OR @product_load_type_id = 5)
						BEGIN
							--Fetch CC, if no card centre use null
							DECLARE @card_centre_id int	= null							

							SELECT TOP 1 @card_centre_id = branch_id, @issuer_id = issuer_id
							FROM [branch]
							WHERE branch_type_id = 0  AND 
								issuer_id = (SELECT TOP 1 issuer_id
											 FROM [branch] 
													INNER JOIN [cards]
														ON [branch].branch_id = [cards].branch_id
													INNER JOIN [load_batch_cards]
														ON [cards].card_id = [load_batch_cards].card_id
											 WHERE [load_batch_cards].load_batch_id = @load_batch_id)
							
							--Create new production batch
							INSERT INTO [dist_batch] (branch_id, no_cards, date_created, dist_batch_reference, 
														card_issue_method_id, dist_batch_type_id, issuer_id)
							 VALUES (@card_centre_id, 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), @card_issue_method_id, 0, @issuer_id)

							 SET @dist_batch_id = SCOPE_IDENTITY();

							 --Link cards
							 INSERT INTO [dist_batch_cards] (dist_batch_id, card_id, dist_card_status_id)
							 SELECT @dist_batch_id, card_id, 0
							 FROM [load_batch_cards]
							 WHERE load_batch_id = @load_batch_id

							 --Find the number of cards
							 SELECT @number_of_dist_cards = COUNT(card_id)
							 FROM [dist_batch_cards]
							 WHERE dist_batch_id = @dist_batch_id

							--Update batch reference
							--Generate dist batch reference
							IF(@product_load_type_id = 1)
								BEGIN
									SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
																[branch].branch_code + '' + 
																CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + '' +
																CAST(@dist_batch_id AS varchar(max))
									FROM [branch] INNER JOIN [issuer]
										ON [branch].issuer_id = [issuer].issuer_id
									WHERE [branch].branch_id = @card_centre_id
								END
							ELSE
								BEGIN
									SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
																CONVERT(VARCHAR(MAX), [cards].product_id) + '' +										  
																CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + '' +
																CAST(@dist_batch_id AS varchar(max))
									FROM [load_batch_cards] 
										INNER JOIN [cards] ON [load_batch_cards].card_id = cards.card_id
										INNER JOIN [branch] ON [branch].branch_id = [cards].branch_id
										INNER JOIN [issuer] ON [branch].issuer_id = [issuer].issuer_id
								END

							--UPDATE dist batch with reference and number of cards
							UPDATE [dist_batch]
							SET [dist_batch_reference] = @dist_batch_ref,
								[no_cards] = @number_of_dist_cards
							WHERE [dist_batch].dist_batch_id = @dist_batch_id

							--Add status of created
							INSERT INTO dist_batch_status(dist_batch_id, dist_batch_statuses_id, [user_id], status_date, status_notes)
							VALUES (@dist_batch_id, 0, @audit_user_id, SYSDATETIMEOFFSET(), 'Batch Created')

							--UPDATE the load batch cards status to allocated							
							UPDATE [load_batch_cards]
							SET [load_batch_cards].load_card_status_id = 2
							WHERE [load_batch_cards].card_id IN 
									(SELECT [dist_batch_cards].card_id
										FROM [dist_batch_cards]
										WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id)

						
							SELECT @dist_batch_status_name =  dist_batch_status_name
							FROM dist_batch_statuses
							WHERE dist_batch_statuses_id = 0

						
							IF(@product_load_type_id = 5)
							BEGIN
								-- Assign card requests at branch to assigned to batch
								-- UPDATE branch card status for those cards that have been added to the new dist batch.
								UPDATE t
								SET t.branch_id = t.branch_id, 
									t.branch_card_statuses_id = 10, 
									t.status_date = SYSDATETIMEOFFSET(), 
									t.[user_id] = @audit_user_id, 
									t.operator_user_id = NULL,
									t.branch_card_code_id = NULL,
									t.comments = 'Assigned to batch',
									t.pin_auth_user_id = NULL
								OUTPUT Deleted.* INTO branch_card_status_audit
								FROM branch_card_status t 
										INNER JOIN [dist_batch_cards] s ON t.card_id = s.card_id
								WHERE s.dist_batch_id = @dist_batch_id

								--INSERT INTO [branch_card_status]
								--	(branch_card_statuses_id, card_id, comments, status_date, [user_id])
								--SELECT 10, card_id, 'Assigned to batch', SYSDATETIMEOFFSET(), @audit_user_id
								--FROM dist_batch_cards
								--WHERE dist_batch_id = @dist_batch_id	
							END

							--Add audit for dist batch creation								
							SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
												', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
												', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
							--log the audit record		
							EXEC usp_insert_audit @audit_user_id, 
													2,
													NULL, 
													@audit_workstation, 
													@audit_msg, 
													NULL, NULL, NULL, NULL

							--Check we've done it all correctly


							--AUTO-APPROVE


						END
					--STEP TWO-b: Create Distribution Batch
					ELSE IF(@product_load_type_id = 2 OR @product_load_type_id = 6)
						BEGIN	

							--Get a distinct list of branches from the load batch
							DECLARE @branch_id int,
									@cards_total int = 0

							--Loop through all distinct branches for the load batch and create dist batches
							DECLARE branchId_cursor CURSOR FOR 
								SELECT DISTINCT [cards].branch_id
								FROM [cards] 
									INNER JOIN [load_batch_cards]
										ON [cards].card_id = [load_batch_cards].card_id
								WHERE [load_batch_cards].load_batch_id = @load_batch_id
						

							OPEN branchId_cursor

							FETCH NEXT FROM branchId_cursor 
							INTO @branch_id

							WHILE @@FETCH_STATUS = 0
							BEGIN

								--IF(SELECT [issuer].auto_create_dist_batch
								--	FROM [issuer] INNER JOIN [branch]
								--			ON [issuer].issuer_id = [branch].issuer_id
								--	WHERE [branch].branch_id = @branch_id) = 1
								--	BEGIN

										--create the distribution batch
										INSERT INTO [dist_batch]
											([issuer_id], [branch_id], [no_cards],[date_created],[dist_batch_reference], 
												[card_issue_method_id],	[dist_batch_type_id])
										SELECT issuer_id, @branch_id, 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), @card_issue_method_id, 1
										FROM [branch]
										WHERE branch_id = @branch_id
										--SELECT
										--	@branch_id, 0, GETDATE(), GETDATE()
										--FROM [load_batch]
										--WHERE [load_batch].load_batch_id = @load_batch_id

										SET @dist_batch_id = SCOPE_IDENTITY();

										--Add cards to distribution batch
										INSERT INTO [dist_batch_cards]
											([dist_batch_id],[card_id],[dist_card_status_id])
										SELECT
											@dist_batch_id, [cards].card_id, 0
										FROM [cards] INNER JOIN [load_batch_cards]
											ON [cards].card_id = [load_batch_cards].card_id
										WHERE [load_batch_cards].load_batch_id = @load_batch_id
												AND [cards].branch_id = @branch_id
							
										--Get the number of cards inserted
										SELECT @number_of_dist_cards = @@ROWCOUNT										

										--add dist batch status of created
										INSERT INTO [dbo].[dist_batch_status]
											([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
										VALUES(@dist_batch_id, 0, @audit_user_id, SYSDATETIMEOFFSET(), 'Auto Dist Batch Create')

										--Generate dist batch reference									
										SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
																  [branch].branch_code + '' + 
																  CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + '' +
																  CAST(@dist_batch_id AS varchar(max))
										FROM [branch] INNER JOIN [issuer]
											ON [branch].issuer_id = [issuer].issuer_id
										WHERE [branch].branch_id = @branch_id

										--UPDATE dist batch with reference and number of cards
										UPDATE [dist_batch]
										SET [dist_batch_reference] = @dist_batch_ref,
											[no_cards] = @number_of_dist_cards
										WHERE [dist_batch].dist_batch_id = @dist_batch_id

										--UPDATE the load batch cards status to allocated							
										UPDATE [load_batch_cards]
										SET [load_batch_cards].load_card_status_id = 2
										WHERE [load_batch_cards].card_id IN 
												(SELECT [dist_batch_cards].card_id
												 FROM [dist_batch_cards]
												 WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id)

										SELECT @dist_batch_status_name =  dist_batch_status_name
										FROM dist_batch_statuses
										WHERE dist_batch_statuses_id = 0

										--Add audit for dist batch creation								
										SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
														 ', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
														 ', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
										--log the audit record		
										EXEC usp_insert_audit @audit_user_id, 
															 2,
															 NULL, 
															 @audit_workstation, 
															 @audit_msg, 
															 NULL, NULL, NULL, NULL

										IF(@auto_approve = 1)
										BEGIN
											--auto add approve the distbatch
											UPDATE dist_batch_status 
												SET [dist_batch_statuses_id] = 1, 
													[user_id] = @audit_user_id, 
													[status_date] = DATEADD(ss,1,SYSDATETIMEOFFSET()), 
													[status_notes] = 'Auto Dist Batch Create Aproval'	
											OUTPUT Deleted.* INTO dist_batch_status_audit
											WHERE [dist_batch_id] = @dist_batch_id

											--INSERT INTO [dbo].[dist_batch_status]
											--	([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
											--VALUES(@dist_batch_id, 1, @audit_user_id, DATEADD(ss,1,SYSDATETIMEOFFSET()), 'Auto Dist Batch Create Aproval')								

											SELECT @dist_batch_status_name =  dist_batch_status_name
											FROM dist_batch_statuses
											WHERE dist_batch_statuses_id = 1

											--Add audit for dist batch update								
											SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
															 ', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
															 ', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
											--log the audit record		
											EXEC usp_insert_audit @audit_user_id, 
																 2,
																 NULL, 
																 @audit_workstation, 
																 @audit_msg, 
																 NULL, NULL, NULL, NULL
										END

								--	END
								--ELSE
								--	BEGIN
								--		--Update the cards linked to the load batch and cursors branch with the available status.
								--		UPDATE [load_batch_cards]
								--		SET load_card_status_id = 1
								--		FROM [load_batch_cards] INNER JOIN [cards]
								--			 ON [load_batch_cards].card_id = [cards].card_id
								--		WHERE [load_batch_cards].load_batch_id = @load_batch_id
								--				AND [cards].branch_id = @branch_id

								--		--Get the number of cards updated
								--		SELECT @number_of_dist_cards = @@ROWCOUNT
								--	END						

								SELECT @cards_total += @number_of_dist_cards

									-- Get the next branch.
								FETCH NEXT FROM branchId_cursor 
								INTO @branch_id
								END 
							CLOSE branchId_cursor;
							DEALLOCATE branchId_cursor;

							--Check that all cards for the load batch have been updated
							IF (SELECT COUNT(card_id) FROM [load_batch_cards] WHERE load_batch_id = @load_batch_id) != @cards_total
							BEGIN
								RAISERROR ('Not all cards have been approved.',
											12,
											12 );
							END
						END
					--STEP TWO-c: Create Card Centre Card Stock
					ELSE IF(@product_load_type_id = 3)
						BEGIN
							UPDATE [load_batch_cards] SET [load_card_status_id]=1 WHERE load_batch_id=@load_batch_id 
							-- WHERE card_id=(SELECT avail_cc_and_load_cards.card_id FROM avail_cc_and_load_cards
							--				INNER JOIN load_batch_cards on avail_cc_and_load_cards.card_id = load_batch_cards.card_id
							--				WHERE load_batch_id=@load_batch_id )
						END
					--STEP TWO-d: Update Production Batch
					ELSE IF(@product_load_type_id = 4)
						BEGIN
							UPDATE [dist_batch_cards]
							SET dist_card_status_id = 21
							WHERE card_id IN (SELECT card_id
												FROM [load_batch_cards]
												WHERE load_batch_id = @load_batch_id)

							
							UPDATE [dist_batch_status]
								SET [dist_batch_statuses_id] = 21, 
									[user_id] = @audit_user_id, 
									[status_date] = SYSDATETIMEOFFSET(), 
									[status_notes] = 'All cards loaded'	
							OUTPUT Deleted.* INTO dist_batch_status_audit
							WHERE [dist_batch_id] IN (
								SELECT DISTINCT dbc.dist_batch_id
								FROM [dist_batch] dbc
									INNER JOIN [dist_batch_cards]
										ON [dist_batch_cards].dist_batch_id = dbc.dist_batch_id
									INNER JOIN [load_batch_cards]
										ON [dist_batch_cards].card_id = [load_batch_cards].card_id
								WHERE 21 = ALL(SELECT dist_card_status_id 
												FROM [dist_batch_cards] dbc2 						
												WHERE dbc2.dist_batch_id = dbc.dist_batch_id)
										AND [load_batch_cards].load_batch_id = @load_batch_id
							)

							--INSERT INTO [dist_batch_status] (dist_batch_id, dist_batch_statuses_id, status_date, status_notes, [user_id])
							--SELECT DISTINCT dbc.dist_batch_id, 21, SYSDATETIMEOFFSET(), 'All cards loaded', @audit_user_id
							--FROM [dist_batch] dbc
							--	INNER JOIN [dist_batch_cards]
							--		ON [dist_batch_cards].dist_batch_id = dbc.dist_batch_id
							--	INNER JOIN [load_batch_cards]
							--		ON [dist_batch_cards].card_id = [load_batch_cards].card_id
							--WHERE 21 = ALL(SELECT dist_card_status_id 
							--				FROM [dist_batch_cards] dbc2 						
							--				WHERE dbc2.dist_batch_id = dbc.dist_batch_id)
							--		AND [load_batch_cards].load_batch_id = @load_batch_id
						END					
					ELSE
						RAISERROR ('Unkown load batch type.', 12, 12);


					--STEP THREE: Approve load batch
					--Update the load batch status.
					UPDATE load_batch_status
						SET [load_batch_statuses_id] = 1,
							[user_id] = @audit_user_id, 
							[status_date] = SYSDATETIMEOFFSET(), 
							[status_notes] = @status_notes
					OUTPUT Deleted.* INTO load_batch_status_audit
					WHERE [load_batch_id] = @load_batch_id

					--INSERT load_batch_status
					--		([load_batch_id], [load_batch_statuses_id], [user_id], [status_date], [status_notes])
					--VALUES (@load_batch_id, 1, @audit_user_id, SYSDATETIMEOFFSET(), @status_notes)
	
					--STEP FOUR: Log Audit
					--log the audit record for load batch
					DECLARE @load_batch_ref varchar(100),
							@load_batch_status varchar(50)

					SELECT @load_batch_ref = load_batch_reference
					FROM [load_batch]
					WHERE load_batch_id = @load_batch_id

					SELECT @load_batch_status = load_batch_status_name
					FROM [load_batch_statuses]
					WHERE load_batch_statuses_id = 1
						
					SET @audit_msg = 'Update: ' + CAST(@load_batch_id AS varchar(max)) + 
									 ', ' + COALESCE(@load_batch_ref, 'UNKNWON') +
									 ', ' + COALESCE(@load_batch_status, 'UNKNOWN')

					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
										 5, --LoadBatch
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0

					COMMIT TRANSACTION [APPROVE_LOAD_BATCH]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [APPROVE_LOAD_BATCH]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END