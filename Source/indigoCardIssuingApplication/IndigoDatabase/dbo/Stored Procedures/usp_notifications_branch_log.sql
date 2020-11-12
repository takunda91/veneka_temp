-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_notifications_branch_log]
	@message_list AS dbo.[notification_array] READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [LOG_BRANCH_NOTIF]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--Store messages
			INSERT INTO [notification_branch_log] (added_time, card_id, issuer_id, branch_card_statuses_id, channel_id, notification_text)
			SELECT SYSDATETIMEOFFSET(), card_id, issuer_id, branch_card_statuses_id, channel_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),msg_list.message_text)) 
			FROM [notification_branch_outbox]
				INNER JOIN @message_list msg_list
					ON [notification_branch_outbox].branch_message_id = msg_list.message_id
	 
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--Remove them from outbox
			DELETE FROM [notification_branch_outbox]
			WHERE branch_message_id IN (SELECT message_id FROM @message_list)

			COMMIT TRANSACTION [LOG_BRANCH_NOTIF]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [LOG_BRANCH_NOTIF]
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