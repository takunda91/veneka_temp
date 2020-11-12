-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_notifications_batch_log]
	@message_list AS dbo.[notification_array] READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [LOG_BATCH_NOTIF]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--Store messages
			INSERT INTO [notification_batch_log] (added_time, dist_batch_id, issuer_id, dist_batch_statuses_id, channel_id, notification_text)
			SELECT SYSDATETIMEOFFSET(), dist_batch_id, issuer_id, dist_batch_statuses_id, channel_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),msg_list.message_text)) 
			FROM [notification_batch_outbox]
				INNER JOIN @message_list msg_list
					ON [notification_batch_outbox].batch_message_id = msg_list.message_id
	 
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--Remove them from outbox
			DELETE FROM [notification_batch_outbox]
			WHERE batch_message_id IN (SELECT message_id FROM @message_list)

			COMMIT TRANSACTION [LOG_BATCH_NOTIF]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [LOG_BATCH_NOTIF]
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