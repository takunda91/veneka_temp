-- =============================================
-- Author:		LTladi
-- Create date: 20150213
-- Description:	Update Masterkey
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_masterkey]
	@masterkey_id int 
	, @masterkey varchar(max)
	, @masterkey_name varchar(250)
	, @issuer_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @ResultCode int =null OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRANSACTION [UPDATE_MASTERKEY_TRAN]
		BEGIN TRY 
		
				OPEN Symmetric Key Indigo_Symmetric_Key
				DECRYPTION BY Certificate Indigo_Certificate;
				IF (SELECT COUNT(*) FROM [masterkeys] WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) =@masterkey and masterkey_id != @masterkey_id) > 0
				BEGIN
					
					SET @ResultCode = 606						
				END
			ELSE	
		
				IF (SELECT COUNT(*) FROM [masterkeys] WHERE [masterkeys].masterkey_name =@masterkey_name and masterkey_id != @masterkey_id) > 0
				BEGIN
					
					SET @ResultCode = 607						
				END
			ELSE			
			BEGIN
				UPDATE [dbo].[masterkeys]
				SET [masterkey] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@masterkey))
				  ,[issuer_id] = @issuer_id
				  ,[masterkey_name] = @masterkey_name 
				  ,[date_changed] = SYSDATETIMEOFFSET()
				WHERE masterkey_id = @masterkey_id

				SET @ResultCode = 0	

				CLOSE Symmetric Key Indigo_Symmetric_Key;

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@issuer_name varchar(100)

				SELECT @issuer_name = issuer_name
				FROM [issuer]
				WHERE issuer_id = @issuer_id

				SET @audit_description = 'Masterkey Update: ' + CAST(@masterkey_name AS VARCHAR(max))	+ 
										+', id :'+  CAST(@masterkey_id AS VARCHAR(max)) 
										--+ ', issuer: ' + COALESCE(@issuer_name, 'UNKNOWN') 
										 	
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @issuer_id, NULL, NULL, NULL	
					END

			COMMIT TRANSACTION [UPDATE_MASTERKEY_TRAN]				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_MASTERKEY_TRAN]
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