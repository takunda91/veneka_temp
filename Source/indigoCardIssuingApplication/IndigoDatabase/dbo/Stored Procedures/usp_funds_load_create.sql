USE [indigo_database_2.1.4.0]
GO

/****** Object:  StoredProcedure [dbo].[usp_funds_load_create]    Script Date: 2019/11/06 16:09:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	create entry for funds_load
-- =============================================
CREATE PROCEDURE [dbo].[usp_funds_load_create] 
	-- Add the parameters for the stored procedure here
	@issuer_id	int,
	@branch_id	int,
	@product_id	int,
	@bank_account_no nvarchar(100), 
	@prepaid_card_no nvarchar(100), 
	@prepaid_account_no nvarchar(100) = null, 
	@amount decimal(18,2), 
	@status int, 
	@creator_id int, 
	@create_date datetime,
	@funds_load_id bigint output, 
	@ResultCode	int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
			OPEN Symmetric Key Indigo_Symmetric_Key
			DECRYPTION BY Certificate Indigo_Certificate;

	BEGIN TRANSACTION [INSERT_FUNDS_LOAD]
		BEGIN TRY 

			-- Insert statements for procedure here
			insert into funds_load (
					[issuer_id],
					[branch_id],
					[product_id],
					[bank_account_no], 
					[prepaid_card_no], 
					[prepaid_account_no], 
					[amount], 
					[status], 
					[creator_id], 
					[create_date]
					)
			values (
					@issuer_id,
					@branch_id,
					@product_id,
					CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@bank_account_no))), 
					CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@prepaid_card_no))), 
					CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@prepaid_account_no))), 
					@amount, 
					@status, 
					@creator_id, 
					@create_date
					)

				SET @funds_load_id = SCOPE_IDENTITY();
				SET @ResultCode = 0			
	
			COMMIT TRANSACTION [INSERT_FUNDS_LOAD]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_FUNDS_LOAD]
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
GO


