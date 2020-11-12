-- =============================================
-- Author:		LTladi	
-- Create date: 20151116
-- Description:	Insert customer_fields into db
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_customer_fields]
	@product_field_id bigint
	, @value VARCHAR(MAX)
	, @audit_user_id bigint
	, @audit_workstation varchar(100)
	, @customer_fields_id bigint OUTPUT
	, @ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	OPEN Symmetric Key Indigo_Symmetric_Key
	DECRYPTION BY Certificate Indigo_Certificate;

	BEGIN TRANSACTION [INSERT_CUSTOMER_FIELDS_TRAN]
	BEGIN TRY 
		INSERT INTO [dbo].[customer_fields]
			   ([product_field_id]
			   ,[value])
		 VALUES
			   (@product_field_id
			   , CONVERT(VARBINARY(MAX),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,@value))))
		   
		SET @customer_fields_id = SCOPE_IDENTITY();
		SET @ResultCode = 0	
		

		--log the audit record
		DECLARE @audit_description varchar(max), @audit_branch_name varchar(max), 
				@audit_branch_code varchar(max)

		SET @audit_description = 'Customer Print Fields Create: Product Print Id '+ COALESCE(@product_field_id, 'UNKNOWN') 
									+ ' , Customer Field Id: ' + CAST(@customer_fields_id AS VARCHAR(MAX))
										 	
		EXEC usp_insert_audit @audit_user_id, 
								9,
								NULL, 
								@audit_workstation, 
								@audit_description, 
								NULL, NULL, NULL, NULL

								SET @ResultCode = 0		

		COMMIT TRANSACTION [INSERT_CUSTOMER_FIELDS_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_CUSTOMER_FIELDS_TRAN]
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