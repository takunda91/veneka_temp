-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject a production batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_reject_production] 
	@dist_batch_id bigint,
	@status_notes varchar(150),
	@reject_card_list AS dbo.key_value_array READONLY,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_PROD_BATCH]
		BEGIN TRY 
			
			DECLARE @current_dist_batch_status_id int,
					@audit_msg varchar(max),
					@status_date datetimeoffset = SYSDATETIMEOFFSET()

			--get the current status for the distribution batch
			SELECT @current_dist_batch_status_id = dist_batch_statuses_id
			FROM dist_batch_status
			WHERE status_date = (SELECT MAX(status_date)
								 FROM dist_batch_status
								 WHERE dist_batch_id = @dist_batch_id)
				  AND dist_batch_id = @dist_batch_id
										  
			--Check that someone hasn't already updated the dist batch
			IF(@current_dist_batch_status_id != 0)
				BEGIN
					SET @ResultCode = 1
				END
			ELSE
				BEGIN
					DECLARE @reject_dist_batch_statuses_id int
					SET @reject_dist_batch_statuses_id = 8

					--Update the dist batch status.
					UPDATE dist_batch_status 
					SET [dist_batch_statuses_id] = @reject_dist_batch_statuses_id, 
						[user_id] = @audit_user_id, 
						[status_date] = @status_date, 
						[status_notes] = @status_notes	
					OUTPUT Deleted.* INTO dist_batch_status_audit
					WHERE [dist_batch_id] = @dist_batch_id

					--INSERT dist_batch_status
					--		([dist_batch_id], [dist_batch_statuses_id], [user_id], [status_date], [status_notes])
					--VALUES (@dist_batch_id, @reject_dist_batch_statuses_id, @audit_user_id, @status_date, @status_notes)

					--Update the cards linked to the prod batch with the new status.
					UPDATE dist_batch_cards
					SET dist_card_status_id = 7
					WHERE dist_batch_id = @dist_batch_id

					--Return valid cards to pool
					UPDATE t
					SET t.branch_id = t.branch_id, 
						t.branch_card_statuses_id = 3, 
						t.status_date = @status_date, 
						t.[user_id] = @audit_user_id, 
						t.operator_user_id = NULL,
						t.branch_card_code_id = NULL,
						t.comments = NULL,
						t.pin_auth_user_id = NULL
					OUTPUT Deleted.* INTO branch_card_status_audit
					FROM branch_card_status t INNER JOIN [dist_batch_cards] s
							ON t.card_id = s.card_id
					WHERE s.dist_batch_id = @dist_batch_id AND
							s.card_id NOT IN (SELECT [key] FROM @reject_card_list)

					--INSERT INTO branch_card_status
					--	(card_id, comments, status_date, branch_card_statuses_id, [user_id])
					--SELECT [dist_batch_cards].card_id, '', @status_date, 3, @audit_user_id
					--FROM [dist_batch_cards]
					--WHERE dist_batch_id = @dist_batch_id AND
					--	[dist_batch_cards].card_id NOT IN (SELECT [key] FROM @reject_card_list)

					--Put rejected cards back to operators to do list
					UPDATE t
					SET t.branch_id = t.branch_id, 
						t.branch_card_statuses_id = 11, 
						t.status_date = @status_date, 
						t.[user_id] = @audit_user_id, 
						t.operator_user_id = NULL,
						t.branch_card_code_id = NULL,
						t.comments = reject_list.value,
						t.pin_auth_user_id = NULL
					OUTPUT Deleted.* INTO branch_card_status_audit
					FROM branch_card_status t 
							INNER JOIN [dist_batch_cards] s	ON t.card_id = s.card_id
							INNER JOIN @reject_card_list reject_list ON s.card_id = reject_list.[key]
					WHERE s.dist_batch_id = @dist_batch_id

					--INSERT INTO branch_card_status
					--	(card_id, comments, status_date, branch_card_statuses_id, [user_id])
					--SELECT [dist_batch_cards].card_id, reject_list.value, @status_date, 11, @audit_user_id
					--FROM [dist_batch_cards] 
					--		INNER JOIN @reject_card_list reject_list
					--			ON [dist_batch_cards].card_id = reject_list.[key]
					--WHERE dist_batch_id = @dist_batch_id
					
					--Audit record stuff
					DECLARE @dist_batch_status_name varchar(50),
							@dist_batch_ref varchar(100)

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = @reject_dist_batch_statuses_id

					SELECT @dist_batch_ref = dist_batch_reference
					FROM dist_batch
					WHERE dist_batch_id = @dist_batch_id

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

					SET @ResultCode = 0					
				END

				--Fetch the distribution batch with latest details
				EXEC usp_get_dist_batch @dist_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [REJECT_PROD_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_PROD_BATCH]
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