-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi	
-- Create date: 20150212
-- Description:	Create a new Masterkey
-- =============================================
CREATE PROCEDURE sp_create_masterkey
	@masterkey varchar(max)
	, @issuer_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @new_masterkey_id int OUTPUT
	, @ResultCode int OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	    -- Insert statements for procedure here

	BEGIN TRANSACTION [INSERT_MASTERKEY_TRAN]
		BEGIN TRY 

			--Check for duplicate's
			IF (SELECT COUNT(*) FROM [masterkeys] WHERE [masterkeys].masterkey = CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@masterkey)))) > 0
				BEGIN
					SET @new_masterkey_id = 0
					SET @ResultCode = 200						
				END
			ELSE			
			BEGIN

			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;

			INSERT INTO [dbo].[masterkeys]
				   ([masterkey]
				   ,[issuer_id]
				   ,[date_created]
				   ,[date_changed])
			 VALUES
				   (CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@masterkey)))
				   ,@issuer_id
				   ,GETDATE()
				   ,GETDATE())

		   	SET @new_masterkey_id = SCOPE_IDENTITY();
			
			CLOSE Symmetric Key Indigo_Symmetric_Key;

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@issuer_name varchar(100)

				SELECT @issuer_name = issuer_name
				FROM [issuer]
				WHERE issuer_id = @issuer_id

				SET @audit_description = 'Create: id: ' + CAST(@new_masterkey_id AS VARCHAR(max))	+ 
										 ', issuer: ' + COALESCE(@issuer_name, 'UNKNOWN') 
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 0,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @new_masterkey_id, NULL, NULL, NULL

									 SET @ResultCode = 0		
					
			END

			COMMIT TRANSACTION [INSERT_MASTERKEY_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_MASTERKEY_TRAN]
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
