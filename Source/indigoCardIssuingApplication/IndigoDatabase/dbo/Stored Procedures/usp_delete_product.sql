
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_delete_product]
	@productid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@result_code int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	BEGIN TRANSACTION [DELETE_PRODUCT_TRAN]
		BEGIN TRY 

			-- delete interface connections first
			DELETE FROM product_interface WHERE product_id = @productid

			
				
			-- then disable the product
			UPDATE issuer_product set DeletedYN=1 where product_id=@productid

			--Hard delete if no cards are linked to product
			IF(NOT EXISTS (SELECT product_id from cards where product_id = @productid))
			BEGIN
				DELETE FROM product_fields
				WHERE product_id = @productid

				DELETE FROM integration_cardnumbers
				WHERE product_id = @productid

				DELETE FROM product_external_system
				WHERE product_id = @productid

				DELETE FROM product_issue_reason
				WHERE product_id = @productid

				DELETE FROM products_account_types
				WHERE product_id = @productid

				DELETE FROM product_currency
				WHERE product_id = @productid

				DELETE FROM issuer_product
				WHERE product_id = @productid

			
				--AND NOT EXISTS (SELECT product_id from cards where product_id = @productid)
			END		


			-- then update the audit information
			DECLARE @audit_description varchar(max), @issuer_id int
			SELECT @audit_description = 'Product Deleted: ' + CAST(@productid as varchar(max))
										+' , Product Id: ' + CAST(@productid as varchar(max))

			IF EXISTS(SELECT product_id from cards where product_id = @productid)
				SET @audit_description += ' , Product soft deleted.'
			ELSE
				SET @audit_description += ' , Product hard deleted.'

			SELECT 	@issuer_id = issuer_id
			FROM [issuer_product]
			WHERE product_id = @productid
																	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

			COMMIT TRANSACTION [DELETE_PRODUCT_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [DELETE_PRODUCT_TRAN]
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