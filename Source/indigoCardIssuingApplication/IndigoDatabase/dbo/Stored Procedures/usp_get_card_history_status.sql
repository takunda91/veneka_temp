-- =============================================
-- Author:		LTladi
-- Create date: 20150520
-- Description:	Card View History Page: Get batch status history
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_history_status]
	@card_id INT
	, @languageId INT = NULL
	, @audit_user_id BIGINT
	, @audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		DECLARE @UserTimezone nvarchar(50) = [dbo].[GetUserTimeZone](@audit_user_id);

		BEGIN TRANSACTION [GET_CARD_HIST_STATUS]
			BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT
				db.dist_batch_id
				, dist_batch_reference
				, dbt.dist_batch_type_name 
				, CAST(SWITCHOFFSET(db.date_created, @UserTimezone) AS DATETIME) as 'date_created'
			FROM
				dist_batch db
				LEFT JOIN dist_batch_cards dbc ON db.dist_batch_id = dbc.dist_batch_id
				LEFT JOIN cards c ON c.card_id = dbc.card_id
				LEFT JOIN dist_batch_type dbt ON dbt.dist_batch_type_id = db.dist_batch_type_id
			WHERE
				dbc.card_id = @card_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			COMMIT TRANSACTION [GET_CARD_HIST_STATUS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_HIST_STATUS]
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