-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_notification_branch]
@issuer_id int ,
@branch_card_statuses_id int ,
@card_issue_method_id int ,
@channel_id int,
@audit_user_id bigint,
@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	    BEGIN TRANSACTION [DELETE_NOTIFICATIONS_BRANCH_TRAN]
		BEGIN TRY 
			DELETE FROM notification_branch_messages WHERE issuer_id=@issuer_id AND
													card_issue_method_id=@card_issue_method_id AND 
													branch_card_statuses_id=@branch_card_statuses_id AND
													channel_id=@channel_id

			COMMIT TRANSACTION [DELETE_NOTIFICATIONS_BRANCH_TRAN]

  END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_NOTIFICATIONS_BRANCH_TRAN]
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