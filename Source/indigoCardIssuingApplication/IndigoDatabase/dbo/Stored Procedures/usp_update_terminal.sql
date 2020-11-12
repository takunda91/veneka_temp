-- =============================================
-- Author:		LTladi
-- Create date: 20150130
-- Description:	Update the terminal information
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_terminal]
	@terminal_id int
	, @terminal_name varchar(250)
	, @terminal_model varchar(250)
	, @device_id varchar(max)
	, @branch_id int
	, @terminal_masterkey_id int
	,@password   varchar(max)
	,@IsMacUsed bit
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;
	BEGIN TRANSACTION [UPDATE_TERMINAL_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [terminals] WHERE ([terminals].[terminal_name] = @terminal_name AND [terminals].[terminal_id] != @terminal_id)) > 0
				BEGIN					
					SET @ResultCode = 604						
				END
			ELSE IF (SELECT COUNT(*) FROM [terminals] WHERE (CONVERT(VARCHAR(max),DECRYPTBYKEY([terminals].[device_id]))  = @device_id AND [terminals].[terminal_id] != @terminal_id)) > 0
				BEGIN
					SET @ResultCode = 605
				END
			ELSE			
			BEGIN
			
		

				UPDATE [dbo].[terminals]
				   SET [terminal_name] = @terminal_name
					  ,[terminal_model] = @terminal_model
					  ,[device_id] = CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@device_id)))
					  ,[branch_id] = @branch_id
					  ,[terminal_masterkey_id] = @terminal_masterkey_id
					  ,[workstation] = @audit_workstation
					  ,[date_changed] = SYSDATETIMEOFFSET()
					  ,[password]=CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@password)))
						,IsMacUsed=@IsMacUsed
				 WHERE [terminal_id] = @terminal_id
				 SET @ResultCode = 0
			

				--log the audit record
				DECLARE @audit_description varchar(max), @issuer_id int, @audit_branch_name varchar(max), 
						@audit_branch_code varchar(max)

				SELECT @issuer_id = issuer_id, @audit_branch_name = branch_name,
						@audit_branch_code = branch_code
				FROM [branch]
				WHERE branch_id = @branch_id

				SET @audit_description = 'Terminal Updated: '+ COALESCE(@terminal_name, 'UNKNOWN') 
										  + ' , Terminal Id: ' + CAST(@terminal_id as varchar(max))
										  + ' , branch code: ' + @audit_branch_code
										  + ' , branch name: ' + @audit_branch_name		

				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL
					
			END

			COMMIT TRANSACTION [UPDATE_TERMINAL_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_TERMINAL_TRAN]
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
	CLOSE Symmetric Key Indigo_Symmetric_Key;	
END