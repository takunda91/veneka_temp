-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_notification_batch] 
@issuer_id int ,
@dist_batch_type_id int ,
@dist_batch_statuses_id int ,
@channel_id INT,
@notifications_lang_messages AS notifications_lang_messages READONLY,
@audit_user_id bigint,
@audit_workstation varchar(100),
@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  

		BEGIN TRY 
		--IF (SELECT COUNT(dist_batch_type_id) FROM notification_batch_messages WHERE (dist_batch_type_id = @dist_batch_type_id AND channel_id=@channel_id AND  [issuer_id] = @issuer_id and language_id=0)) >1
		--		BEGIN
		--			SET @ResultCode = 805						
		--		END
		--	ELSE 
			IF (SELECT COUNT(dist_batch_type_id) FROM notification_batch_messages WHERE ( dist_batch_type_id = @dist_batch_type_id and dist_batch_statuses_id = @dist_batch_statuses_id AND channel_id=@channel_id AND [issuer_id] = @issuer_id and language_id=0)) > 1
				BEGIN
					SET @ResultCode = 806
				END
			ELSE
			BEGIN
			  BEGIN TRANSACTION [UPDATE_NOTIFICATIONS_BATCH_TRAN]
			UPDATE notification_batch_messages
                         SET issuer_id =@issuer_id, 
						 dist_batch_type_id=@dist_batch_type_id, 
						 dist_batch_statuses_id=@dist_batch_statuses_id,
						  language_id =nlm.language_id, 
						  channel_id=@channel_id, 
						  from_address=nlm.from_address,

						  notification_text=nlm.notification_text,
						   subject_text=nlm.subject_text		
					FROM @notifications_lang_messages  nlm
					INNER JOIN notification_batch_messages ON  nlm.language_id =notification_batch_messages.language_id
					and notification_batch_messages.issuer_id=@issuer_id
					and  dist_batch_type_id=@dist_batch_type_id 
					and dist_batch_statuses_id=@dist_batch_statuses_id
					SET @ResultCode = 0

			COMMIT TRANSACTION [UPDATE_NOTIFICATIONS_BATCH_TRAN]
			END
  END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_NOTIFICATIONS_BATCH_TRAN]
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
GO

