-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_notification_batch]
@issuer_id int ,
@dist_batch_type_id int ,
@dist_batch_statuses_id int ,
@channel_id int,
@audit_user_id bigint,
@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	    BEGIN TRANSACTION [DELETE_NOTIFICATIONS_BATCH_TRAN]
		BEGIN TRY 
			DELETE FROM notification_batch_messages WHERE issuer_id=@issuer_id AND
													dist_batch_type_id=@dist_batch_type_id AND 
													dist_batch_statuses_id=@dist_batch_statuses_id AND
													channel_id=@channel_id

			COMMIT TRANSACTION [DELETE_NOTIFICATIONS_BATCH_TRAN]

  END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_NOTIFICATIONS_BATCH_TRAN]
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