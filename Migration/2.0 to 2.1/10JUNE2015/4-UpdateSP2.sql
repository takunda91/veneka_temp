USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_dist_batch_status_change]    Script Date: 2015-06-10 02:13:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change batch status - Change
-- =============================================
ALTER PROCEDURE [dbo].[sp_dist_batch_status_change] 
	@dist_batch_id bigint,
	@dist_batch_statuses_id int,
	@new_dist_batch_statuses_id int,
	@status_notes varchar(150) = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
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
						
			--If the statuses are the same then we arent changing the batches status, just return dist batch
			--IF(@dist_batch_statuses_id = @new_dist_batch_statuses_id)	
			--	BEGIN
			--		SET @ResultCode = 0	
			--	END							  
			--Check that someone hasn't already updated the dist batch
			IF(dbo.DistBatchInCorrectStatus(@dist_batch_statuses_id, @new_dist_batch_statuses_id, @dist_batch_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN	
					--Check if we need to create dist batch
					SELECT @original_batch_type_id = [dist_batch_statuses_flow].dist_batch_type_id,
							  @new_batch_type_id = flow_dist_batch_type_id,
							  @new_dist_card_statuses_id = flow_dist_card_statuses_id,
							  @new_branch_card_statuses_id = branch_card_statuses_id
						FROM [dist_batch_statuses_flow]
							INNER JOIN [dist_batch]
								ON [dist_batch_statuses_flow].card_issue_method_id = [dist_batch].card_issue_method_id
									AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
							INNER JOIN [dist_batch_status_current]
								ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_flow].dist_batch_statuses_id
									AND [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
					WHERE [dist_batch].dist_batch_id = @dist_batch_id
					

					--Update the batch status.
					INSERT [dist_batch_status]
							([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					VALUES (@dist_batch_id, @new_dist_batch_statuses_id, @audit_user_id, GETDATE(), @status_notes)


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
									INSERT INTO [branch_card_status] (branch_card_statuses_id, card_id, status_date, [user_id])
									SELECT @new_branch_card_statuses_id, [dist_batch_cards].card_id, GETDATE(), @audit_user_id
									FROM [dist_batch_cards]
									WHERE dist_batch_id = @dist_batch_id
								END
							END						
					END
					
					--Do we need to create a distribution batch from a production batch
					IF(@original_batch_type_id = 0 AND @new_batch_type_id = 1)
					BEGIN
						EXEC sp_prod_to_dist_batch @dist_batch_id, @audit_user_id, @audit_workstation
					END

					--PINS_PRINTED check if we need to create pin mailer batch
					IF (@new_dist_batch_statuses_id = 18)
					BEGIN
						EXEC sp_prod_to_pin @dist_batch_id, @audit_user_id, @audit_workstation
					END

					--RECEIVED_AT_BRANCH needs to add cards to branch_card_status
					IF (@new_dist_batch_statuses_id = 3)
					BEGIN
						EXEC sp_dist_batch_to_vault @dist_batch_id, @audit_user_id, @audit_workstation
					END

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
					EXEC sp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0					
				END

				--Fetch the batch with latest details
				EXEC sp_get_dist_batch @dist_batch_id,
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








