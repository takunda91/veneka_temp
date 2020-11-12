
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_product_fee_accounting] 
	-- Add the parameters for the stored procedure here
	@fee_accounting_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),	
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Check that fee schemes arent using this fee_accounting_id
	IF (SELECT COUNT(*) FROM [product_fee_scheme] WHERE [fee_accounting_id] = @fee_accounting_id) > 0
		BEGIN
			SET @ResultCode = 230						
		END
	ELSE
	BEGIN 
		BEGIN TRANSACTION [DELETE_ACCOUNTING_TRAN]
		BEGIN TRY 

				DELETE FROM[dbo].[product_fee_accounting]			   
				WHERE [fee_accounting_id] = @fee_accounting_id

				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Fee Accounting Deleted: Id: ' + CAST(@fee_accounting_id as varchar(max))

				EXEC usp_insert_audit @audit_user_id, 
										9,
										NULL, 
										@audit_workstation, 
										@audit_description, 
										NULL, NULL, NULL, NULL		

				SET @ResultCode = 0
				COMMIT TRANSACTION [DELETE_ACCOUNTING_TRAN]

			END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION [DELETE_ACCOUNTING_TRAN]
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