-- =============================================
-- Author:		LTladi
-- Create date: 20150520
-- Description:	Card View History Page: Get batch reference numbers
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_history_reference]
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

	BEGIN TRANSACTION [GET_CARD_HIST]
		BEGIN TRY 

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT
				[branch_card_statuses_name]
				, CAST(SWITCHOFFSET(status_date, @UserTimezone) as datetime) as 'status_date'
				, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(u.username)) AS username
			FROM 
				cards c
				LEFT JOIN branch_card_status cs ON c.card_id = cs.card_id				
				LEFT JOIN branch_card_statuses bcs ON cs.branch_card_statuses_id = bcs.branch_card_statuses_id
				LEFT JOIN [user] u ON u.[user_id] = cs.[user_id]
			WHERE
				c.card_id = @card_id
			UNION ALL
			SELECT
				[branch_card_statuses_name]
				, CAST(SWITCHOFFSET(status_date, @UserTimezone) as datetime) as 'status_date'
				, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(u.username)) AS username
			FROM 
				cards c				
				LEFT JOIN branch_card_status_audit csa ON csa.card_id = c.card_id
				LEFT JOIN branch_card_statuses bcs ON csa.branch_card_statuses_id = bcs.branch_card_statuses_id
				LEFT JOIN [user] u ON u.[user_id] = csa.[user_id]
			WHERE
				c.card_id = @card_id
			ORDER BY status_date

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	

			COMMIT TRANSACTION [GET_CARD_HIST]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_HIST]
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