-- =============================================
-- Author:		sandhya
-- Create date: 
-- Description:	Insert link between users group and branches
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_product_currency] 
	-- Add the parameters for the stored procedure here
	@product_id int,
	@currencylist as dbo.product_currency_array READONLY,
	
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--BEGIN TRANSACTION [INSERT_USER_GROUP_BRANCHES_TRAN]

	--	BEGIN TRY

			INSERT INTO product_currency
						(product_id, currency_id, is_base, usr_field_name_1, usr_field_val_1, usr_field_name_2, usr_field_val_2)
			SELECT @product_id, cl.currency_id, is_base, usr_field_name_1, usr_field_val_1, usr_field_name_2, usr_field_val_2
			FROM @currencylist cl

			--Insert audit train
			--EXEC usp_insert_audit @audit_user_id, 
			--						 21,
			--						 NULL, 
			--						 @audit_workstation, 
			--						 'Linking branches to user group.', 
			--						 NULL, NULL, NULL, NULL

	--		COMMIT TRANSACTION [INSERT_USER_GROUP_BRANCHES_TRAN]

	--	END TRY
	--BEGIN CATCH
	--	ROLLBACK TRANSACTION [INSERT_USER_GROUP_BRANCHES_TRAN]
	--	DECLARE @ErrorMessage NVARCHAR(4000);
	--	DECLARE @ErrorSeverity INT;
	--	DECLARE @ErrorState INT;

	--	SELECT 
	--		@ErrorMessage = ERROR_MESSAGE(),
	--		@ErrorSeverity = ERROR_SEVERITY(),
	--		@ErrorState = ERROR_STATE();

	--	RAISERROR (@ErrorMessage, -- Message text.
	--			   @ErrorSeverity, -- Severity.
	--			   @ErrorState -- State.
	--			   );
	--END CATCH 	

END