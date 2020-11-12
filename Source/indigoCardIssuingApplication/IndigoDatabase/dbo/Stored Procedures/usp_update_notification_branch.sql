-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_notification_branch] 
@issuer_id int ,
@branch_card_statuses_id int ,
@card_issue_method_id int ,
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
	BEGIN TRANSACTION [UPDATE_NOTIFICATIONS_BRANCH_TRAN]
		BEGIN TRY 
		--IF (SELECT COUNT(*) FROM notification_branch_messages WHERE (card_issue_method_id = @card_issue_method_id AND channel_id=@channel_id AND [issuer_id] = @issuer_id and language_id=0)) > 1
		--		BEGIN
		--			SET @ResultCode = 803						
		--		END
		--	ELSE 
			IF (SELECT COUNT(*) FROM notification_branch_messages WHERE (card_issue_method_id = @card_issue_method_id and branch_card_statuses_id = @branch_card_statuses_id AND channel_id=@channel_id AND [issuer_id] = @issuer_id and language_id=0)) > 1
				BEGIN
					SET @ResultCode = 804
				END
			ELSE
			BEGIN
				UPDATE notification_branch_messages
                         SET issuer_id =@issuer_id, 
						 card_issue_method_id=@card_issue_method_id, 
						 branch_card_statuses_id=@branch_card_statuses_id,
						  language_id =nlm.language_id, 
						  channel_id=@channel_id, 
						  from_address=nlm.from_address,
						  notification_text=nlm.notification_text,
						   subject_text=nlm.subject_text		
					FROM @notifications_lang_messages  nlm
					INNER JOIN notification_branch_messages ON  nlm.language_id =notification_branch_messages.language_id
					and notification_branch_messages.issuer_id=@issuer_id
					and notification_branch_messages.branch_card_statuses_id=@branch_card_statuses_id
					SET @ResultCode = 0


		COMMIT TRANSACTION [UPDATE_NOTIFICATIONS_BRANCH_TRAN]
		END
  END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_NOTIFICATIONS_BRANCH_TRAN]
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

