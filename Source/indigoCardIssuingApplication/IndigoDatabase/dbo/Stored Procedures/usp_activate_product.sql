
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_activate_product]
	@productid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@result_code int =null output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN TRANSACTION [ACTIVATE_PRODUCT_TRAN]
		BEGIN TRY 
			-- then disable the product
			UPDATE issuer_product set DeletedYN=0 where product_id=@productid

			-- then update the audit information
			DECLARE @audit_description varchar(max), @issuer_id int
			SELECT @audit_description = 'Product Activated: ' + CAST(@productid as varchar(max))
										+' , Product Id: ' + CAST(@productid as varchar(max))

			SELECT 	@issuer_id = issuer_id
			FROM [issuer_product]
			WHERE product_id = @productid
																	
			EXEC usp_insert_audit @audit_user_id, 
									4,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									@issuer_id, NULL, NULL, NULL

			COMMIT TRANSACTION [ACTIVATE_PRODUCT_TRAN]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [ACTIVATE_PRODUCT_TRAN]
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