USE [indigo_database_2.1.4.0]
GO

/****** Object:  StoredProcedure [dbo].[usp_funds_load_list_prepaid_card]    Script Date: 2019/11/06 16:07:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	Get card info
-- =============================================
CREATE  PROCEDURE [dbo].[usp_funds_load_list_prepaid_card] 
	@prepaid_card_no nvarchar(100),
	@check_masking BIT,
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [GET_FUNDS_LOAD]
		BEGIN TRY 

			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)			
			Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

			declare @applyMask bit = 0
			if (@check_masking=1 and @mask_screen=1)
			begin
				set @applyMask = 1
			end
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--@check_masking = This proc is used by some backend process that require a clear mask and therefore 
			-- overrides the @mask_screen checking.

				SELECT distinct 
						f.funds_load_id
					   , CASE @applyMask
							WHEN 1 THEN 
									[dbo].[MaskString](CONVERT(VARCHAR(100),DECRYPTBYKEY(f.bank_account_no)),6,4) 
									ELSE CONVERT(VARCHAR(100),DECRYPTBYKEY(f.bank_account_no))
						 END AS 'bank_account_no'
					   , CASE @applyMask
							WHEN 1 THEN 
									[dbo].[MaskString](CONVERT(VARCHAR(100),DECRYPTBYKEY(f.prepaid_card_no)),6,4) 
									ELSE CONVERT(VARCHAR(100),DECRYPTBYKEY(f.prepaid_card_no))
						 END AS 'prepaid_card_no'
					   , f.amount
					   , f.[status]
				FROM dbo.[funds_load] f
				WHERE f.prepaid_card_no = CONVERT(varbinary(max),ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,@prepaid_card_no)))
				

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key


			COMMIT TRANSACTION [GET_FUNDS_LOAD]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_FUNDS_LOAD]
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


