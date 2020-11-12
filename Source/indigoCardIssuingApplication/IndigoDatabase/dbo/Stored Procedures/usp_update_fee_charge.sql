-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_fee_charge] 
	-- Add the parameters for the stored procedure here
	@fee_detail_id int, 
	@fee_list  as dbo.[fee_charge_array] READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),	
	@ResultCode int = null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

       -- Insert statements for procedure here
	BEGIN TRANSACTION [UPDATE_PRODUCT_FEE_CHARGE_TRAN]
		BEGIN TRY 			

			DELETE FROM [product_fee_charge]
			WHERE fee_detail_id = @fee_detail_id

			INSERT INTO [product_fee_charge] (fee_detail_id, card_issue_reason_id,cbs_account_type, currency_id, fee_charge, vat, date_created)
			SELECT @fee_detail_id,fl.card_issue_reason_id,fl.cbs_account_type, fl.[currency_id], fl.[fee_charge], fl.[vat], GETUTCDATE()
			FROM @fee_list fl	


			--log the audit record
			DECLARE @audit_description varchar(max), @issuer_id int, @fees varchar(max) = ''

			SELECT  @fees = STUFF(
									(SELECT ', ' + [currency].currency_code + '=' +  CONVERT(varchar(max), fee_list.[fee_charge]) + '- vat=' +  CONVERT(varchar(max), fee_list.[vat])+ 'card reason '+ CONVERT(varchar(max),fee_list.card_issue_reason_id)
									 FROM @fee_list fee_list
										INNER JOIN [currency]
											ON [currency].currency_id = fee_list.[currency_id]
									 FOR XML PATH(''))
								   , 1
								   , 1
								   , '')


			SELECT @issuer_id = issuer_id
			FROM [product_fee_detail] INNER JOIN [product_fee_scheme]
				ON [product_fee_scheme].fee_scheme_id = [product_fee_detail].fee_scheme_id
			WHERE [product_fee_detail].fee_detail_id = @fee_detail_id
					

			SET @audit_description = 'Fee Charge Update: Fee Detail Id ' + CAST(@fee_detail_id AS VARCHAR(max))	
										+ ', Fees: ' + COALESCE(@fees, 'NONE')
										 	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

			SET @ResultCode = 0	

			COMMIT TRANSACTION [UPDATE_PRODUCT_FEE_CHARGE_TRAN]
			SET @ResultCode = 0
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PRODUCT_FEE_CHARGE_TRAN]
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







