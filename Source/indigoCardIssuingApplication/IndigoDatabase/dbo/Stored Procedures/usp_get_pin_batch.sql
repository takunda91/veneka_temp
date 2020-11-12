
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Retreive a pin batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_batch] 
	@pin_batch_id bigint,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	BEGIN TRANSACTION [GET_PIN_BATCH]
		BEGIN TRY 

			SELECT CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES, 
				   [pin_batch].pin_batch_id, CAST(SWITCHOFFSET( [pin_batch].date_created,@UserTimezone) as DateTime) as 'date_created' , [pin_batch].pin_batch_reference, 
				   [pin_batch].no_cards, [pin_batch_status_current].pin_batch_statuses_id, 
				   [pin_batch_status_current].status_notes, [pin_batch_statuses_language].language_text AS 'pin_batch_status_name', 
				   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				   [card_issue_method_language].language_text AS 'card_issue_method_name',
				   [pin_batch].card_issue_method_id, [pin_batch].pin_batch_type_id,
				   [branch].branch_name, [branch].branch_code,
				   [pin_batch_statuses_flow].flow_pin_batch_statuses_id, 
				   [pin_batch_statuses_flow].flow_pin_batch_type_id,
				   [pin_batch_statuses_flow].user_role_id,
				   [pin_batch_statuses_flow].reject_pin_batch_statuses_id  
			FROM [pin_batch] 
				INNER JOIN [pin_batch_status_current]
					ON [pin_batch].pin_batch_id = [pin_batch_status_current].pin_batch_id
				INNER JOIN [pin_batch_statuses_language]
					ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_language].pin_batch_statuses_id							
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [pin_batch].issuer_id
				INNER JOIN [card_issue_method_language]
					ON [pin_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
						AND [card_issue_method_language].language_id = @language_id	
						
				LEFT OUTER JOIN [branch]
					ON [branch].branch_id = [pin_batch].branch_id

				LEFT OUTER JOIN [pin_batch_statuses_flow]
					ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
						AND [pin_batch].card_issue_method_id = [pin_batch_statuses_flow].card_issue_method_id
						AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id
			WHERE [pin_batch].pin_batch_id = @pin_batch_id
					AND [pin_batch_statuses_language].language_id = @language_id		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting dist batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_PIN_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH]
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