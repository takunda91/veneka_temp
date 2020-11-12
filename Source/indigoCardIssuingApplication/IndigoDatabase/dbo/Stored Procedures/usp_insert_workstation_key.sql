CREATE PROCEDURE [dbo].[usp_insert_workstation_key]
@workstation varchar(500),
@key varchar(500),
@additional_data varchar(500)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


   BEGIN TRANSACTION [INSERT_MASTERKEY_TRAN]
		BEGIN TRY 

		

			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;
			 if((SELECT  count(*) from workstation_keys	where CONVERT(VARCHAR(100),DECRYPTBYKEY(workstation))=@workstation)=0)
				
				BEGIN
					INSERT INTO workstation_keys(workstation, [key],additional_data)
					VALUES        (ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@workstation)),
								ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@key)),
								ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@additional_data))
								)
								
				END
				
			 CLOSE Symmetric Key Indigo_Symmetric_Key;
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

