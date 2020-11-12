CREATE PROCEDURE [dbo].[usp_get_pin_batch_history] 
	@pin_batch_id bigint,	
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_PIN_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;
		Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

			SELECT 
				pb.pin_batch_reference
				, pb.no_cards
				, CAST(SWITCHOFFSET(pbs.status_date,@UserTimezone) as DATETIME) as status_date
				 
				, '' AS status_notes -- [pin_batch_status].status_notes
				, pbsl.language_text as pin_batch_status_name
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
			FROM 
				[pin_batch] pb
				INNER JOIN [pin_batch_status] pbs ON pbs.pin_batch_id = pb.pin_batch_id
				INNER JOIN pin_batch_statuses pbss ON pbs.pin_batch_statuses_id = pbss.pin_batch_statuses_id
				INNER JOIN pin_batch_statuses_language pbsl ON pbsl.pin_batch_statuses_id = pbs.pin_batch_statuses_id
				INNER JOIN [user] ON pbs.[user_id] = [user].[user_id]
			WHERE 
				pb.pin_batch_id = @pin_batch_id
				AND pbsl.language_id = @languageId
			GROUP BY	
				pb.pin_batch_reference
				, pb.no_cards
				, pbs.status_date
				, pbsl.language_text
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username]))
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) 
				, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name))
			ORDER BY 
				pbs.status_date ASC	

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

			COMMIT TRANSACTION [GET_PIN_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH_HISTORY]
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