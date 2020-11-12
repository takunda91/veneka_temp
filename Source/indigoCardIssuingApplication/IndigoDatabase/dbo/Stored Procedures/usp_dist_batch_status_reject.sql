
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Change batch status - Reject
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_status_reject] 
	@dist_batch_id bigint,
	@new_dist_batch_status_id int,
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

	BEGIN TRANSACTION [BATCH_STATUS_CHANGE_REJECT]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@new_dist_card_statuses_id int,
					@new_branch_card_statuses_id int

			--Check that someone hasn't already updated the dist batch
			IF(dbo.DistBatchInCorrectStatusReject(@new_dist_batch_status_id, @dist_batch_id, @audit_user_id) = 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					SELECT @new_dist_card_statuses_id = reject_dist_card_statuses_id,
							@new_branch_card_statuses_id = reject_branch_card_statuses_id
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



					--	FROM [dist_batch_statuses_flow]
					--		INNER JOIN [dist_batch]
					--			ON [dist_batch_statuses_flow].card_issue_method_id = [dist_batch].card_issue_method_id
					--				AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
					--		INNER JOIN [dist_batch_status_current]
					--			ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_flow].dist_batch_statuses_id
					--				AND [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
					--WHERE [dist_batch].dist_batch_id = @dist_batch_id
					--		AND [dist_batch_statuses_flow].reject_dist_batch_statuses_id = @new_dist_batch_status_id


					--Update the batch status.
					UPDATE dist_batch_status 
					SET [dist_batch_statuses_id] = @new_dist_batch_status_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes	
					OUTPUT Deleted.* INTO dist_batch_status_audit
					WHERE [dist_batch_id] = @dist_batch_id

					--INSERT [dist_batch_status]
					--		([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					--VALUES (@dist_batch_id, @new_dist_batch_status_id, @audit_user_id, SYSDATETIMEOFFSET(), @status_notes)

					--Check if we need to update the card status
					IF (@new_dist_card_statuses_id IS NOT NULL)
					BEGIN 
						--Update the cards linked to the dist batch with the new status.
						UPDATE dist_batch_cards
						SET dist_card_status_id = @new_dist_card_statuses_id
						WHERE dist_batch_id = @dist_batch_id

						--Going back to checked in or destroying, make sure cards are back at originating branch
						IF(@new_dist_card_statuses_id = 18 OR @new_dist_card_statuses_id = 11)
						BEGIN
							UPDATE [cards]
							SET branch_id = [cards].origin_branch_id
							FROM [cards]
								INNER JOIN [dist_batch_cards]
									ON [cards].card_id = [dist_batch_cards].card_id
							WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id 
						END
					END

					IF(@new_branch_card_statuses_id IS NOT NULL)
					BEGIN
						UPDATE t
						SET t.branch_id = t.branch_id, 
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
						WHERE s.dist_batch_id = @dist_batch_id

						--INSERT INTO [branch_card_status] (branch_card_statuses_id, card_id, status_date, [user_id])
						--SELECT @new_branch_card_statuses_id, [dist_batch_cards].card_id, SYSDATETIMEOFFSET(), @audit_user_id
						--FROM [dist_batch_cards]
						--WHERE dist_batch_id = @dist_batch_id
						
					END
				
					--AUDIT
					DECLARE @batch_status_name varchar(100),
							@batch_ref varchar(100)

					SELECT @batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @new_dist_batch_status_id

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

				COMMIT TRANSACTION [BATCH_STATUS_CHANGE_REJECT]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [BATCH_STATUS_CHANGE_REJECT]
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