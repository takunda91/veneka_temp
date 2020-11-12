CREATE PROCEDURE [dbo].[usp_get_print_batch] 
	@print_batch_id bigint,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	BEGIN TRANSACTION [GET_PRINT_BATCH]
		BEGIN TRY 

			SELECT CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES, 
				   [print_batch].print_batch_id, CAST(SWITCHOFFSET( [print_batch].date_created,@UserTimezone) as DateTime) as 'date_created' , [print_batch].print_batch_reference, 
					COALESCE( [print_batch].no_of_requests,0) as  'no_cards', [print_batch_status_current].print_batch_statuses_id, 
				   [print_batch_status_current].status_notes, [print_batch_statuses_language].language_text AS 'print_batch_status_name', 
				   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				   [card_issue_method_language].language_text AS 'card_issue_method_name',
				   [print_batch].card_issue_method_id, hybrid_requests.product_id,
				   [branch].branch_name, [branch].branch_code
				   --[print_batch_statuses_flow].flow_print_batch_statuses_id, 
				   --[print_batch_statuses_flow].flow_print_batch_type_id,
				   --[print_batch_statuses_flow].user_role_id,
				   --[print_batch_statuses_flow].reject_print_batch_statuses_id  
			FROM [print_batch] 
				INNER JOIN [print_batch_status_current]
					ON [print_batch].print_batch_id = [print_batch_status_current].print_batch_id
				INNER JOIN [print_batch_statuses_language]
					ON [print_batch_status_current].print_batch_statuses_id = [print_batch_statuses_language].print_batch_statuses_id							
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [print_batch].issuer_id
					INNER JOIN [print_batch_requests] on [print_batch].print_batch_id=[print_batch_requests].print_batch_id
				INNER JOIN hybrid_requests
				 ON [print_batch_requests].request_id=hybrid_requests.request_id
				INNER JOIN [card_issue_method_language]
					ON [print_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
						AND [card_issue_method_language].language_id = @language_id	
						
				LEFT OUTER JOIN [branch]
					ON [branch].branch_id = [print_batch].branch_id

				--LEFT OUTER JOIN [print_batch_statuses_flow]
				--	ON [print_batch_status_current].print_batch_statuses_id = [print_batch_statuses_flow].print_batch_statuses_id
				--		AND [print_batch].card_issue_method_id = [print_batch_statuses_flow].card_issue_method_id
				--		AND [print_batch_statuses_flow].print_batch_type_id = [print_batch].print_batch_type_id
			WHERE [print_batch].print_batch_id = @print_batch_id
					AND [print_batch_statuses_language].language_id = @language_id		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting dist batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_PRINT_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PRINT_BATCH]
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


