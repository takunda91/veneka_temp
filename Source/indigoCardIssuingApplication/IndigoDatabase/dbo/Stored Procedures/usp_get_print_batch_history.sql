CREATE PROCEDURE [dbo].[usp_get_print_batch_history] 
	@print_batch_id bigint,	
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	BEGIN TRANSACTION [GET_PRINT_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT 
					[print_batch].print_batch_reference
					, COALESCE( [print_batch].no_of_requests,0) as no_requests			
					
					,CAST(SWITCHOFFSET(print_batch_status.status_date,@UserTimezone) as DATETIME) as status_date
					, print_batch_status.status_notes
					, [print_batch_statuses_language].language_text as print_batch_status_name
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
					, branch.branch_code
					, branch.branch_name
				FROM [print_batch]
						INNER JOIN print_batch_status
							ON [print_batch].print_batch_id = [print_batch_status].print_batch_id
						INNER JOIN [print_batch_statuses]
							ON [print_batch_status].print_batch_statuses_id = [print_batch_statuses].print_batch_statuses_id
							INNER JOIN [print_batch_statuses_language]
							ON [print_batch_status].print_batch_statuses_id = [print_batch_statuses_language].print_batch_statuses_id
						INNER JOIN [user]
							ON [print_batch_status].[user_id] = [user].[user_id]
						LEFT JOIN [branch]
							ON [branch].branch_id = [print_batch].branch_id
				WHERE [print_batch].print_batch_id = @print_batch_id and [print_batch_statuses_language].language_id=@languageId
				ORDER BY status_date ASC	

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting history for distribution batch with id: ' + CAST(@dist_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_PRINT_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PRINT_BATCH_HISTORY]
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










