-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_mailer_reprint_approve] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@comment nvarchar(1000),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [TRAN_PIN_REPRINT_APPROVE]
		BEGIN TRY 
			
			DECLARE @audit_msg varchar(max),
					@pin_mailer_reprint_id bigint,
					@new_batch_type_id int,
					@new_dist_card_statuses_id int

			IF ( (SELECT COUNT(*) FROM [pin_mailer_reprint_status_current] 
					WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id != 0 AND 
						  [pin_mailer_reprint_status_current].card_id = @card_id) > 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN

					DECLARE @pin_mailer_reprint_status_id int = 1
					--Insert create pin request
					UPDATE [pin_mailer_reprint]
						SET [user_id] = @audit_user_id
							,[pin_mailer_reprint_status_id] = @pin_mailer_reprint_status_id
							,[status_date] = SYSDATETIMEOFFSET()
							,[comments] = @comment
					OUTPUT Deleted.* INTO  [pin_mailer_reprint_audit]
					WHERE [card_id] = @card_id

					--INSERT INTO [pin_mailer_reprint] (card_id, comments, pin_mailer_reprint_status_id, status_date, [user_id])
					--	VALUES (@card_id, @comment, @pin_mailer_reprint_status_id, SYSDATETIMEOFFSET(), @audit_user_id)
					
					--Add audit for pin reprint update								
					SET @audit_msg = 'create: ' + CAST(@card_id AS varchar(max)) +
										', pin mailer reprint approved by ' + CAST(@audit_user_id AS varchar(max)) 
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					 

					SET @ResultCode = 0	
				END	

				COMMIT TRANSACTION [TRAN_PIN_REPRINT_APPROVE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [TRAN_PIN_REPRINT_APPROVE]
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