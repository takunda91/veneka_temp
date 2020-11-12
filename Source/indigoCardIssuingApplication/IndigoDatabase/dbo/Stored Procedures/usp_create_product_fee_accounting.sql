
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_product_fee_accounting] 
	-- Add the parameters for the stored procedure here
	@fee_accounting_name varchar(150),
    @issuer_id int,
    @fee_revenue_account_no varchar(100),
    @fee_revenue_account_type_id int,
	@fee_revenue_branch_code varchar(10),
    @fee_revenue_narration_en varchar(150),
    @fee_revenue_narration_fr varchar(150),
    @fee_revenue_narration_pt varchar(150),
    @fee_revenue_narration_es varchar(150),
    @vat_account_no varchar(100),
    @vat_account_type_id int,
    @vat_account_branch_code varchar(10),
    @vat_narration_en varchar(150),
    @vat_narration_fr varchar(150),
    @vat_narration_pt varchar(150),
    @vat_narration_es varchar(150),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_fee_accounting_id int OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (SELECT COUNT(*) FROM [product_fee_accounting] WHERE [fee_accounting_name] = @fee_accounting_name AND [issuer_id] = @issuer_id) > 0
		BEGIN
			SET @new_fee_accounting_id = 0
			SET @ResultCode = 229						
		END
	ELSE
	BEGIN 
		BEGIN TRANSACTION [INSERT_ACCOUNTING_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			INSERT INTO [dbo].[product_fee_accounting]
				   ([fee_accounting_name]
				   ,[issuer_id]
				   ,[fee_revenue_account_no]
				   ,[fee_revenue_account_type_id]
				   ,[fee_revenue_branch_code]
				   ,[fee_revenue_narration_en]
				   ,[fee_revenue_narration_fr]
				   ,[fee_revenue_narration_pt]
				   ,[fee_revenue_narration_es]
				   ,[vat_account_no]
				   ,[vat_account_type_id]
				   ,[vat_account_branch_code]
				   ,[vat_narration_en]
				   ,[vat_narration_fr]
				   ,[vat_narration_pt]
				   ,[vat_narration_es])
			 VALUES
				   (@fee_accounting_name,
					@issuer_id,
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@fee_revenue_account_no)),
					@fee_revenue_account_type_id,
					@fee_revenue_branch_code,
					@fee_revenue_narration_en,
					@fee_revenue_narration_fr,
					@fee_revenue_narration_pt,
					@fee_revenue_narration_es,
					ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@vat_account_no)),
					@vat_account_type_id,
					@vat_account_branch_code,
					@vat_narration_en,
					@vat_narration_fr,
					@vat_narration_pt,
					@vat_narration_es)

				SET @new_fee_accounting_id = SCOPE_IDENTITY();

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key ;--Closes sym key

				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Fee Accounting Created: ' + @fee_accounting_name
											+ ', Id: ' + CAST(@new_fee_accounting_id as varchar(max))

				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_description, 
										NULL, NULL, NULL, NULL		

				SET @ResultCode = 0
				COMMIT TRANSACTION [INSERT_ACCOUNTING_TRAN]

			END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION [INSERT_ACCOUNTING_TRAN]
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
END