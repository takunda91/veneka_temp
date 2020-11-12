-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_notification_branch] 
@issuer_id int ,
@branch_card_statuses_id int ,
@card_issue_method_id int ,
@channel_id int ,
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
	BEGIN TRANSACTION [INSERT_NOTIFICATIONS_BRANCH_TRAN]

		--IF (SELECT COUNT(*) FROM notification_branch_messages WHERE (card_issue_method_id = @card_issue_method_id AND channel_id=@channel_id AND [issuer_id] = @issuer_id and language_id=0)) > 0
		--		BEGIN
		--			SET @ResultCode = 803						
		--		END
		--	ELSE 
			IF (SELECT COUNT(*) FROM notification_branch_messages WHERE (branch_card_statuses_id = @branch_card_statuses_id and card_issue_method_id = @card_issue_method_id AND channel_id=@channel_id AND [issuer_id] = @issuer_id and language_id=0)) > 0
				BEGIN
					SET @ResultCode = 804
				END
			ELSE
			BEGIN
		INSERT INTO notification_branch_messages
                         (issuer_id, card_issue_method_id, branch_card_statuses_id, language_id, channel_id, notification_text, subject_text,from_address)
		SELECT @issuer_id,@card_issue_method_id,@branch_card_statuses_id,nlm.language_id,@channel_id,nlm.notification_text,nlm.subject_text,nlm.from_address
		FROM @notifications_lang_messages  nlm
					SET @ResultCode = 0

		END
		COMMIT TRANSACTION [INSERT_NOTIFICATIONS_BRANCH_TRAN]

  END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_NOTIFICATIONS_BRANCH_TRAN]
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

