-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_pin_reissue] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,
	@product_id int,
	@pan varchar(19), 
	@primary_index_number varbinary(max),
	@mobile_number varchar(20),
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@pin_reissue_type int,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Check for any requests that need to be expired
	EXEC [dbo].[usp_pin_reissue_expiry_check]

	BEGIN TRANSACTION [REQUEST_PIN_REISSUE_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @pin_reissue_id bigint = -1

			--Check there are no outstanding requests for this card
			IF((SELECT COUNT(*) FROM [pin_reissue_status_current] INNER JOIN [pin_reissue] 
					ON [pin_reissue_status_current].pin_reissue_id = [pin_reissue].pin_reissue_id
				WHERE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([pin_reissue].pan)) = @pan
						AND [pin_reissue_status_current].pin_reissue_statuses_id IN (0, 1)) > 0)
				BEGIN
					SET @ResultCode = 802
				END
			ELSE
				BEGIN
					DECLARE @OutputTbl TABLE (pin_reissue_id bigint)
					DECLARE @request_date DATETIMEOFFSET = SYSDATETIMEOFFSET()

					INSERT INTO [pin_reissue] (issuer_id, branch_id, operator_user_id, pan, primary_index_number, product_id, reissue_date, failed, notes, request_expiry,pin_reissue_type_id,mobile_number)
					OUTPUT INSERTED.pin_reissue_id INTO @OutputTbl(pin_reissue_id)
					VALUES (@issuer_id, @branch_id, @audit_user_id, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@pan)),
							@primary_index_number, @product_id, @request_date, 0, '', DATEADD(mi, 60, @request_date),@pin_reissue_type,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@mobile_number)))

					INSERT INTO [pin_reissue_status] (pin_reissue_id, pin_reissue_statuses_id, [user_id], audit_workstation, comments, status_date)
					SELECT pin_reissue_id, 0, @audit_user_id, @audit_workstation, 'PIN reissue requested', @request_date
					FROM @OutputTbl


					DECLARE @pin_reissue_statuses_name varchar(100),
							@audit_msg varchar(max)
							

					SELECT @pin_reissue_id = pin_reissue_id
					FROM @OutputTbl

					SELECT @pin_reissue_statuses_name = pin_reissue_statuses_id
					FROM [pin_reissue_statuses]
					WHERE pin_reissue_statuses_id = 0

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

					SET @audit_msg = '' + COALESCE(@pin_reissue_statuses_name, 'UNKNOWN') + 
											 ', ' + dbo.MaskString(@pan, 6, 4)
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											10,
											NULL, 
											@audit_workstation, 
											@audit_msg,
											@issuer_id, NULL, NULL, NULL

						
					
					SET @ResultCode = 0
				END
				EXEC usp_get_pin_reissue @pin_reissue_id, @language_id, @audit_user_id, @audit_workstation
			COMMIT TRANSACTION [REQUEST_PIN_REISSUE_TRAN]		
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REQUEST_PIN_REISSUE_TRAN]
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

