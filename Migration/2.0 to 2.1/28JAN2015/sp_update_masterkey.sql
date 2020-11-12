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
-- Create date: 20150213
-- Description:	Update Masterkey
-- =============================================
CREATE PROCEDURE sp_update_masterkey
	@masterkey_id int 
	, @masterkey varchar(max)
	, @issuer_id int
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRANSACTION [UPDATE_MASTERKEY_TRAN]
		BEGIN TRY 
		
				OPEN Symmetric Key Indigo_Symmetric_Key
				DECRYPTION BY Certificate Indigo_Certificate;

				UPDATE [dbo].[masterkeys]
				SET [masterkey] = CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@masterkey)))
				  ,[issuer_id] = @issuer_id
				  ,[date_changed] = GETDATE()
				WHERE masterkey_id = @masterkey_id

				CLOSE Symmetric Key Indigo_Symmetric_Key;

				--log the audit record
				DECLARE @audit_description varchar(max) = '',
						@issuer_name varchar(100)

				SELECT @issuer_name = issuer_name
				FROM [issuer]
				WHERE issuer_id = @issuer_id

				SET @audit_description = 'Create: id: ' + CAST(@masterkey_id AS VARCHAR(max))	+ 
										 ', name: ' + COALESCE(@issuer_name, 'UNKNOWN')
										 	
				EXEC sp_insert_audit @audit_user_id, 
									 0,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 @masterkey_id, NULL, NULL, NULL	
					

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
GO
