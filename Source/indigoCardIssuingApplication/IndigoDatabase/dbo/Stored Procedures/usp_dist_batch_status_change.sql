-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change batch status - Change
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_status_change] 
	@dist_batch_id bigint,
	@dist_batch_statuses_id int = null,
	@new_dist_batch_statuses_id int = null,
	@status_notes varchar(150) = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@autogenerate_dist_batch_id bit,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [BATCH_STATUS_CHANGE]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@original_batch_type_id int,
					@new_batch_type_id int,
					@new_dist_card_statuses_id int,
					@new_branch_card_statuses_id int


			--Find the current status of the batch and check if the user can move it to the next status
			IF(@dist_batch_statuses_id is NULL AND @new_dist_batch_statuses_id is NULL)
			BEGIN
				SELECT @dist_batch_statuses_id = [dist_batch_statuses_id], 
						@new_dist_batch_statuses_id = [flow_dist_batch_statuses_id]
				FROM [dist_batch_statuses_flow]
				WHERE dist_batch_status_flow_id =
					(SELECT TOP 1 production_dist_batch_status_flow
						FROM [dist_batch_cards]
							INNER JOIN [cards] ON [cards].card_id = [dist_batch_cards].card_id
							INNER JOIN [issuer_product] ON [issuer_product].product_id = [cards].product_id
						WHERE dist_batch_id = @dist_batch_id)
					AND dist_batch_statuses_id = 
					 (SELECT dist_batch_statuses_id
						FROM [dbo].[dist_batch_status_current]
						WHERE dist_batch_id = @dist_batch_id)
					AND user_role_id IN 
					 (SELECT user_role_id
						FROM [user_group] 
							INNER JOIN [dist_batch] ON ([user_group].issuer_id = [dist_batch].issuer_id OR [user_group].issuer_id = -1)
							INNER JOIN [users_to_users_groups] ON [users_to_users_groups].user_group_id = [user_group].user_group_id
								AND [users_to_users_groups].[user_id] = @audit_user_id)
			END						
						  
			--Check that someone hasn't already updated the dist batch
			IF( dbo.DistBatchInCorrectStatus(@dist_batch_statuses_id, @new_dist_batch_statuses_id, @dist_batch_id, @audit_user_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN	
					--Check if we need to create dist batch
					SELECT @original_batch_type_id = [dist_batch_status_flow].dist_batch_type_id,
							  @new_batch_type_id = flow_dist_batch_type_id,
							  @new_dist_card_statuses_id = flow_dist_card_statuses_id,
							  @new_branch_card_statuses_id = branch_card_statuses_id
					FROM [dist_batch_status_current]
						INNER JOIN [dist_batch]
							ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
						INNER JOIN [dist_batch_cards]
							ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
						INNER JOIN cards  
							ON [dist_batch_cards].card_id = cards.card_id
						INNER JOIN [issuer_product]
							ON cards.product_id = [issuer_product].product_id
						INNER JOIN [dist_batch_statuses_flow] AS [product_flow]
							ON (([dist_batch].dist_batch_type_id = 0 AND 
									[product_flow].dist_batch_status_flow_id = [issuer_product].production_dist_batch_status_flow)
								OR ([dist_batch].dist_batch_type_id = 1 AND 
									[product_flow].dist_batch_status_flow_id = [issuer_product].distribution_dist_batch_status_flow))
								AND [product_flow].dist_batch_statuses_id = [dist_batch_status_current].dist_batch_statuses_id
						INNER JOIN [dist_batch_status_flow]
							ON [dist_batch_status_flow].dist_batch_status_flow_id = [product_flow].dist_batch_status_flow_id
					WHERE dist_batch_status_current.dist_batch_id = @dist_batch_id
					

					--Update the batch status.
					UPDATE dist_batch_status 
					SET [dist_batch_statuses_id] = @new_dist_batch_statuses_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes	
					OUTPUT Deleted.* INTO dist_batch_status_audit
					WHERE [dist_batch_id] = @dist_batch_id

					--INSERT [dist_batch_status]
					--		([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					--VALUES (@dist_batch_id, @new_dist_batch_statuses_id, @audit_user_id, SYSDATETIMEOFFSET(), @status_notes)


					--Receiving at card centre, update originating batch to batchid
					IF(@new_dist_batch_statuses_id = 14)
					BEGIN
						UPDATE [cards]
						SET origin_branch_id = [cards].branch_id
						FROM [cards]
							INNER JOIN [dist_batch_cards]
								ON [cards].card_id = [dist_batch_cards].card_id
						WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
					END


					--Check if we need to update the card status
					IF (@new_dist_card_statuses_id IS NOT NULL)
					BEGIN 
						--Update the cards linked to the dist batch with the new status.
						UPDATE dist_batch_cards
						SET dist_card_status_id = @new_dist_card_statuses_id
						WHERE dist_batch_id = @dist_batch_id

						--Reject status, make sure cards are back at originating branch
						IF(@new_dist_card_statuses_id = 18 OR @new_dist_card_statuses_id = 11 OR
							@new_dist_card_statuses_id = 7)
							BEGIN
								UPDATE [cards]
								SET branch_id = [cards].origin_branch_id
								FROM [cards]
									INNER JOIN [dist_batch_cards]
										ON [cards].card_id = [dist_batch_cards].card_id
								WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id 

								IF(@new_branch_card_statuses_id = 0)
								BEGIN
									UPDATE t
									SET t.branch_id = [cards].branch_id, 
										t.branch_card_statuses_id = @new_branch_card_statuses_id, 
										t.status_date = SYSDATETIMEOFFSET(), 
										t.[user_id] = @audit_user_id, 
										t.operator_user_id = NULL,
										t.branch_card_code_id = NULL,
										t.comments = NULL,
										t.pin_auth_user_id = NULL
									OUTPUT Deleted.* INTO branch_card_status_audit
									FROM branch_card_status t 
											INNER JOIN [dist_batch_cards] s ON t.card_id = s.card_id
											INNER JOIN [cards] ON [cards].card_id = s.card_id
									WHERE s.dist_batch_id = @dist_batch_id

									--INSERT INTO [branch_card_status] (branch_card_statuses_id, branch_id, card_id, status_date, [user_id])
									--SELECT @new_branch_card_statuses_id, [cards].branch_id, [dist_batch_cards].card_id, SYSDATETIMEOFFSET(), @audit_user_id
									--FROM [dist_batch_cards] INNER JOIN [cards]
									--	ON [cards].card_id = [dist_batch_cards].card_id
									--WHERE dist_batch_id = @dist_batch_id
								END
							END						
					END
					
					--Do we need to create a distribution batch from a production batch
					IF(@original_batch_type_id = 0 AND @new_batch_type_id = 1)
					BEGIN
						EXEC usp_prod_to_dist_batch @dist_batch_id, @audit_user_id, @audit_workstation,@autogenerate_dist_batch_id 
					END

					--PINS_PRINTED check if we need to create pin mailer batch
					IF (@new_dist_batch_statuses_id = 18)
					BEGIN
						EXEC usp_prod_to_pin @dist_batch_id, @audit_user_id, @audit_workstation
					END

					--RECEIVED_AT_BRANCH needs to add cards to branch_card_status
					IF (@new_dist_batch_statuses_id = 3)
					BEGIN
						EXEC usp_dist_batch_to_vault @dist_batch_id, @audit_user_id, @audit_workstation

						--Check if the cards should be changed to another status, overriding previous sp.
						IF(@new_branch_card_statuses_id IS NOT NULL)
						BEGIN
							UPDATE t
							SET t.branch_id = [cards].branch_id, 
								t.branch_card_statuses_id = @new_branch_card_statuses_id, 
								t.status_date = DATEADD(s, 1,SYSDATETIMEOFFSET()), 
								t.[user_id] = @audit_user_id, 
								t.operator_user_id = NULL,
								t.branch_card_code_id = NULL,
								t.comments = NULL,
								t.pin_auth_user_id = NULL
							OUTPUT Deleted.* INTO branch_card_status_audit
							FROM branch_card_status t 
									INNER JOIN [dist_batch_cards] s ON t.card_id = s.card_id
									INNER JOIN [cards] ON [cards].card_id = s.card_id
							WHERE s.dist_batch_id = @dist_batch_id

							--INSERT INTO [branch_card_status] (branch_card_statuses_id, branch_id, card_id, status_date, [user_id])
							--SELECT @new_branch_card_statuses_id, [cards].branch_id, [dist_batch_cards].card_id, DATEADD(s, 1,SYSDATETIMEOFFSET()), @audit_user_id
							--FROM [dist_batch_cards] INNER JOIN [cards]
							--	ON [cards].card_id = [dist_batch_cards].card_id
							--WHERE dist_batch_id = @dist_batch_id
						END
					END

					--NOTIFICATION of BATCH
					EXEC usp_notification_batch_add @dist_batch_id, @new_dist_batch_statuses_id					

					--AUDIT 
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @new_dist_batch_statuses_id

					SELECT @batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@dist_batch_id AS varchar(max)) +
										', ' + COALESCE(@batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0					
				END

				--Fetch the batch with latest details
				EXEC usp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [BATCH_STATUS_CHANGE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [BATCH_STATUS_CHANGE]
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
GO


