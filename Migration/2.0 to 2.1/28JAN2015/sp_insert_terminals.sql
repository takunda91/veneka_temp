USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_terminal]    Script Date: 2015/02/10 05:21:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi
-- Create date: 20150130
-- Description:	Insert new terminals (pin pad/POS)
-- =============================================
CREATE PROCEDURE [dbo].[sp_create_terminal]
	@terminal_name varchar(250)
	, @terminal_model varchar(250)
	, @device_id varchar(max)
	, @branch_id int
	, @terminal_masterkey_id int
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

	BEGIN TRANSACTION [INSERT_TERMINAL_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [terminals] WHERE [terminals].[terminal_name] = @terminal_name) > 0
				BEGIN
					SET @new_terminal_id = 0
					SET @ResultCode = 200						
				END
			ELSE IF (SELECT COUNT(*) FROM [terminals] WHERE ([terminals].[device_id] = @device_id)) > 0
				BEGIN
					SET @new_terminal_id = 0
					SET @ResultCode = 201
				END
			ELSE			
			BEGIN

			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;

				INSERT INTO [dbo].[terminals]
					   ( [terminal_name]
					   , [terminal_model]
					   , [device_id]
					   , [branch_id]
					   , [terminal_masterkey_id]
					   , [workstation]
					   , [date_created]
					   , [date_changed])
				 VALUES
					   ( @terminal_name
					   , @terminal_model
					   , CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@device_id)))
					   , @branch_id
					   , @terminal_masterkey_id
					   , @audit_workstation
					   , GETDATE()
					   , GETDATE())

				SET @new_terminal_id = SCOPE_IDENTITY();
			
			CLOSE Symmetric Key Indigo_Symmetric_Key;

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@branch_name varchar(100)

				SELECT @branch_name = branch_name
				FROM [branch]
				WHERE branch_id = @branch_id

				SET @audit_description = 'Create: id: ' + CAST(@new_terminal_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@terminal_name, 'UNKNOWN') +
										 ', model: ' + COALESCE(@terminal_model, 'UNKNOWN') +
										 ', branch: ' + COALESCE(@branch_name, 'UNKNOWN')
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 0,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @new_terminal_id, NULL, NULL, NULL

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
END
