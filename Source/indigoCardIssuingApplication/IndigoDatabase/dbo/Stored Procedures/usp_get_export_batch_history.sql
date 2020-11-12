-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch the distribution batches history
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_export_batch_history] 
	@dist_batch_id bigint,	
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	BEGIN TRANSACTION [GET_EXPORT_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

		
				SELECT 
					[export_batch].batch_reference
					, [export_batch].no_cards
					,CAST(SWITCHOFFSET( [export_batch_status].status_date,@UserTimezone) AS DATETIME) as status_date
					, [export_batch_status].comments
					, [export_batch_statuses_language].language_text as export_batch_status_name
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
					--, branch.branch_code
					--, branch.branch_name
				FROM [export_batch]
						INNER JOIN [export_batch_status]
							ON [export_batch].export_batch_id = [export_batch_status].export_batch_id
						INNER JOIN [export_batch_statuses]
							ON [export_batch_status].export_batch_statuses_id = [export_batch_statuses].export_batch_statuses_id
							INNER JOIN [export_batch_statuses_language]
							ON [export_batch_status].export_batch_statuses_id = [export_batch_statuses_language].export_batch_statuses_id
						INNER JOIN [user]
							ON [export_batch_status].[user_id] = [user].[user_id]
						--LEFT JOIN [branch]
						--	ON [branch].branch_id = [export_batch].branch_id
				WHERE [export_batch].export_batch_id = @dist_batch_id and [export_batch_statuses_language].language_id=@languageId
				ORDER BY [export_batch_status].status_date ASC	

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

			COMMIT TRANSACTION [GET_EXPORT_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_EXPORT_BATCH_HISTORY]
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