-- =============================================
-- Author:		LTladi
-- Create date: 20150130
-- Description:	Insert new terminals (pin pad/POS)
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_terminal]
	@terminal_name varchar(250)
	, @terminal_model varchar(250)
	, @device_id varchar(max)
	, @branch_id int
	, @terminal_masterkey_id int
	,@password   varchar(max)
	,@IsMacUsed bit 
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @new_terminal_id int OUTPUT
	
	, @ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;

	BEGIN TRANSACTION [INSERT_TERMINAL_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [terminals] WHERE [terminals].[terminal_name] = @terminal_name) > 0
				BEGIN
					SET @new_terminal_id = 0
					SET @ResultCode = 604						
				END
			ELSE IF (SELECT COUNT(*) FROM [terminals] WHERE (CONVERT(VARCHAR(max),DECRYPTBYKEY([terminals].[device_id])) = @device_id)) > 0
				BEGIN
					SET @new_terminal_id = 0
					SET @ResultCode = 605
				END
			ELSE			
			BEGIN

		

				INSERT INTO [dbo].[terminals]
					   ( [terminal_name]
					   , [terminal_model]
					   , [device_id]
					   , [branch_id]
					   , [terminal_masterkey_id]
					   ,[password]
					   ,IsMacUsed
					   , [workstation]
					   , [date_created]
					   , [date_changed])
				 VALUES
					   ( @terminal_name
					   , @terminal_model
					   , CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@device_id)))
					   , @branch_id
					   , @terminal_masterkey_id
					   , CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@password)))
					   
					   ,@IsMacUsed
					   , @audit_workstation
					   , SYSDATETIMEOFFSET()
					   , SYSDATETIMEOFFSET())

				SET @new_terminal_id = SCOPE_IDENTITY();
			SET @ResultCode = 0	
		

				--log the audit record
				DECLARE @audit_description varchar(max), @issuer_id int, @audit_branch_name varchar(max), 
						@audit_branch_code varchar(max)

				SELECT @issuer_id = issuer_id, @audit_branch_name = branch_name,
						@audit_branch_code = branch_code
				FROM [branch]
				WHERE branch_id = @branch_id

				SET @audit_description = 'Terminal Create: '+ COALESCE(@terminal_name, 'UNKNOWN') 
										 + ' , Terminal Id: ' + CAST(@new_terminal_id as varchar(max))
										  + ' , branch code: ' + @audit_branch_code
										  + ' , branch name: ' + @audit_branch_name
										 	
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

									 SET @ResultCode = 0		
					
			END

			COMMIT TRANSACTION [INSERT_TERMINAL_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_TERMINAL_TRAN]
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