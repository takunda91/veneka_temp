-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get status hostory for a load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_load_batch_history] 
	@load_batch_id bigint,
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_LOAD_BATCH_HISTORY]
	
Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT [load_batch].load_batch_reference, [load_batch].no_cards,cast(SWITCHOFFSET([load_batch_status].status_date,@UserTimezone)as datetime) as status_date ,
					  [load_batch_statuses_language].language_text as  load_batch_status_name, 
					   CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username',
					   CONVERT(VARCHAR(max),DECRYPTBYKEY([user].first_name)) as 'first_name',
					   CONVERT(VARCHAR(max),DECRYPTBYKEY([user].last_name)) as 'last_name'   
				FROM [load_batch]
						INNER JOIN [load_batch_status]
							ON [load_batch].load_batch_id = [load_batch_status].load_batch_id
						INNER JOIN [load_batch_statuses]
							ON [load_batch_status].load_batch_statuses_id = [load_batch_statuses].load_batch_statuses_id
							INNER JOIN [load_batch_statuses_language]
							ON [load_batch_status].load_batch_statuses_id = [load_batch_statuses_language].load_batch_statuses_id
						INNER JOIN [user]
							ON [load_batch_status].[user_id] = [user].[user_id]
				WHERE [load_batch].load_batch_id = @load_batch_id and [load_batch_statuses_language].language_id=@languageId
				ORDER BY status_date ASC	

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting history for load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_LOAD_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_LOAD_BATCH_HISTORY]
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