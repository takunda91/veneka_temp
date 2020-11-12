-- =============================================
-- Author:		LTladi	
-- Create date: 20150212
-- Description:	Create a new Masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_masterkey]
	@masterkey varchar(max)
	, @masterkey_name varchar(100)
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
			IF (SELECT COUNT(*) FROM [masterkeys] WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) =@masterkey) > 0
				BEGIN
					SET @new_masterkey_id = 0
					SET @ResultCode = 606						
				END
			ELSE	
				IF (SELECT COUNT(*) FROM [masterkeys] WHERE [masterkeys].masterkey_name =@masterkey_name) > 0
				BEGIN
					SET @new_masterkey_id = 0
					SET @ResultCode = 607						
				END
			ELSE			
			BEGIN

			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;

			INSERT INTO [dbo].[masterkeys]
				   ([masterkey]
				   ,[issuer_id]
				   ,[masterkey_name]
				   ,[date_created]
				   ,[date_changed])
			 VALUES
				   (ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@masterkey))
				   ,@issuer_id
				   ,@masterkey_name
				   ,SYSDATETIMEOFFSET()
				   ,SYSDATETIMEOFFSET())

		   	SET @new_masterkey_id = SCOPE_IDENTITY();
			SET @ResultCode =0
			CLOSE Symmetric Key Indigo_Symmetric_Key;

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@issuer_name varchar(100)

				SELECT @issuer_name = issuer_name
				FROM [issuer]
				WHERE issuer_id = @issuer_id

				SET @audit_description = 'MasterKey Create: ' + @masterkey_name	
										+ ', Maskerkey Id: ' + CAST(@new_masterkey_id as varchar(max)) 
										 	
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL

										
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