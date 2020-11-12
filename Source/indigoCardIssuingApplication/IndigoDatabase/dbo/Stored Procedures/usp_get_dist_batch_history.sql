--===========================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch the distribution batches history
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batch_history] 
	@dist_batch_id bigint,	
	@languageId int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	BEGIN TRANSACTION [GET_DIST_BATCH_HISTORY]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;
			SELECT 
					inter.dist_batch_reference
					, inter.no_cards
					,inter.status_date
					, inter.status_notes
					, inter.dist_batch_status_name
					, inter.username
					, inter.first_name
					, inter.last_name 
					, inter.branch_code
					, inter.branch_name
					from 
				(
				(SELECT 
					[dist_batch].dist_batch_reference
					, [dist_batch].no_cards			
					
					,CAST(SWITCHOFFSET([dist_batch_status].status_date,@UserTimezone) as DATETIME) as status_date
					, [dist_batch_status].status_notes
					, [dist_batch_statuses_language].language_text as dist_batch_status_name
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
					, branch.branch_code
					, branch.branch_name
				FROM [dist_batch]
						INNER JOIN [dist_batch_status]
							ON [dist_batch].dist_batch_id = [dist_batch_status].dist_batch_id
						INNER JOIN [dist_batch_statuses]
							ON [dist_batch_status].dist_batch_statuses_id = [dist_batch_statuses].dist_batch_statuses_id
							INNER JOIN [dist_batch_statuses_language]
							ON [dist_batch_status].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
						INNER JOIN [user]
							ON [dist_batch_status].[user_id] = [user].[user_id]
						LEFT JOIN [branch]
							ON [branch].branch_id = dist_batch.branch_id
				WHERE [dist_batch].dist_batch_id = @dist_batch_id and [dist_batch_statuses_language].language_id=@languageId
				)
				union
				
				(SELECT 
					[dist_batch].dist_batch_reference
					, [dist_batch].no_cards			
					
					,CAST(SWITCHOFFSET(dist_batch_status_audit.status_date,@UserTimezone) as DATETIME) as status_date
					, dist_batch_status_audit.status_notes
					, [dist_batch_statuses_language].language_text as dist_batch_status_name
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].[username])) as 'username'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].first_name)) as 'first_name'
					, CONVERT(VARCHAR,DECRYPTBYKEY([user].last_name)) as 'last_name'   
					, branch.branch_code
					, branch.branch_name
				FROM [dist_batch]
						INNER JOIN [dist_batch_status_audit]
							ON [dist_batch].dist_batch_id = dist_batch_status_audit.dist_batch_id
						INNER JOIN [dist_batch_statuses]
							ON dist_batch_status_audit.dist_batch_statuses_id = [dist_batch_statuses].dist_batch_statuses_id
							INNER JOIN [dist_batch_statuses_language]
							ON [dist_batch_status_audit].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
						INNER JOIN [user]
							ON dist_batch_status_audit.[user_id] = [user].[user_id]
						LEFT JOIN [branch]
							ON [branch].branch_id = dist_batch.branch_id
				WHERE [dist_batch].dist_batch_id = @dist_batch_id and [dist_batch_statuses_language].language_id=@languageId
				)			 

				
				) as inter
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

			COMMIT TRANSACTION [GET_DIST_BATCH_HISTORY]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_HISTORY]
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










