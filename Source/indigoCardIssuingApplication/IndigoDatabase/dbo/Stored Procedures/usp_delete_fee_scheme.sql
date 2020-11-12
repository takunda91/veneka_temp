-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_fee_scheme] 
	-- Add the parameters for the stored procedure here
	@fee_scheme_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRY
		BEGIN TRANSACTION [DELETE_MASTERKEY_TRAN]


		IF((SELECT count(*) FROM [issuer_product] WHERE fee_scheme_id = @fee_scheme_id) > 0)
			BEGIN
				SET @ResultCode = 702
			END
		ELSE 
			BEGIN
				DECLARE @audit_product_name varchar(400), @issuer_id int
				SELECT @audit_product_name = fee_scheme_name
				FROM [product_fee_scheme]
				WHERE fee_scheme_id = @fee_scheme_id

				SELECT @issuer_id = issuer_id
				FROM [product_fee_scheme]
				WHERE fee_scheme_id = @fee_scheme_id

				--Delete charges
				DELETE FROM [product_fee_charge]
				WHERE fee_detail_id IN (SELECT fee_detail_id
										FROM [product_fee_detail]
										WHERE fee_scheme_id = @fee_scheme_id)

				--Delete details
				DELETE FROM product_fee_detail
				WHERE fee_scheme_id = @fee_scheme_id

				--Delete the Scheme
				DELETE FROM product_fee_scheme
				WHERE fee_scheme_id = @fee_scheme_id

				--Log in audit trail
				DECLARE @audit_description varchar(500)
				SELECT @audit_description = 'Fee Scheme Deleted: ' + @audit_product_name
											+ ' , Fee Scheme Id: ' + CAST(@fee_scheme_id AS VARCHAR(MAX))		
																	
				EXEC usp_insert_audit @audit_user_id, 
										4,
										NULL, 
										@audit_workstation, 
										@audit_description, 
										@issuer_id, NULL, NULL, NULL	

				SET @ResultCode = 0
			END

			COMMIT TRANSACTION [DELETE_MASTERKEY_TRAN]
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_MASTERKEY_TRAN]
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