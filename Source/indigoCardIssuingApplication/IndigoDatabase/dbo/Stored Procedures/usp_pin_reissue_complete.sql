-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_reissue_complete]
	-- Add the parameters for the stored procedure here
	@pin_reissue_id bigint,
	@notes varchar(100),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [COMPLETE_PIN_REISSUE_TRAN]
		BEGIN TRY 

		DECLARE @auth_pin_reissue bit = 0, 
				@pin_reissue_statuses_id int

		SELECT @pin_reissue_statuses_id = pin_reissue_statuses_id,
				@auth_pin_reissue = [issuer].authorise_pin_reissue_YN
		FROM [pin_reissue_status_current] 
				INNER JOIN [pin_reissue]
					ON [pin_reissue].pin_reissue_id = [pin_reissue_status_current].pin_reissue_id
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [pin_reissue].issuer_id
		WHERE pin_reissue_status_current.pin_reissue_id = @pin_reissue_id

		IF(@auth_pin_reissue = 1 AND @pin_reissue_statuses_id != 1)
			BEGIN
				SET @ResultCode = 800
			END
		ELSE IF(@auth_pin_reissue = 0 AND @pin_reissue_statuses_id != 0)
			BEGIN
				SET @ResultCode = 800
			END
		ELSE
			BEGIN
				UPDATE [pin_reissue_status]
					SET pin_reissue_statuses_id = 3, 
					[user_id] = @audit_user_id, 
					audit_workstation = @audit_workstation, 
					comments = @notes, 
					status_date = SYSDATETIMEOFFSET()
				OUTPUT Deleted.* INTO [pin_reissue_status_audit]
				WHERE pin_reissue_id = @pin_reissue_id

				--INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, [user_id], audit_workstation, comments, status_date)
				--VALUES (@pin_reissue_id, 3, @audit_user_id, @audit_workstation, @notes, GETDATE())

				UPDATE [pin_reissue]
				SET primary_index_number = null
				WHERE pin_reissue_id = @pin_reissue_id
			
				OPEN SYMMETRIC KEY Indigo_Symmetric_Key
				DECRYPTION BY CERTIFICATE Indigo_Certificate

				DECLARE @cardnumber varchar(50),
						@issuer_id int,
						@pin_reissue_statuses_name varchar(100),
						@audit_msg varchar(max)

				SELECT @cardnumber = CONVERT(VARCHAR(max),DECRYPTBYKEY(pan)) ,
						@issuer_id = issuer_id
				FROM [pin_reissue] 
				WHERE pin_reissue_id = @pin_reissue_id

				SELECT @pin_reissue_statuses_name = pin_reissue_statuses_id
				FROM [pin_reissue_statuses]
				WHERE pin_reissue_statuses_id = 3

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

				SET @audit_msg = '' + COALESCE(@pin_reissue_statuses_name, 'UNKNOWN') + 
										 ', ' + dbo.MaskString(@cardnumber, 6, 4)
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										10,
										NULL, 
										@audit_workstation, 
										@audit_msg,
										@issuer_id, NULL, NULL, NULL

				SET @ResultCode = 0
			END

			COMMIT TRANSACTION [COMPLETE_PIN_REISSUE_TRAN]
			EXEC usp_get_pin_reissue @pin_reissue_id, @language_id, @audit_user_id, @audit_workstation			

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [COMPLETE_PIN_REISSUE_TRAN]
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