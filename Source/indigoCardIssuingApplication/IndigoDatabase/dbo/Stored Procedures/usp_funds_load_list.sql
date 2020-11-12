USE [indigo_database_2.1.4.0]
GO

/****** Object:  StoredProcedure [dbo].[usp_funds_load_list]    Script Date: 2019/11/08 12:51:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	Get card info
-- =============================================
CREATE  PROCEDURE [dbo].[usp_funds_load_list] 
	@status int,
	@issuer_id	int = null,
	@branch_id	int = null,
	@check_masking BIT,
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if @issuer_id=0 set @issuer_id=null
	if @branch_id=0 set @branch_id=null
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
					   , i.issuer_code
					   , i.issuer_name
					   , b.branch_code
					   , b.branch_name
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
					   , f.create_date
					   , f.product_id
					   , f.issuer_id
					   , f.branch_id
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY(c.username)) as created_by
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY(r.username)) as reviewed_by
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY(a.username)) as approved_by
					   , CONVERT(VARCHAR(100),DECRYPTBYKEY(l.username)) as loaded_by
				FROM dbo.[funds_load] f
				inner join dbo.issuer i on f.issuer_id=i.issuer_id
				inner join dbo.branch b on f.branch_id=b.branch_id
				left join dbo.[user] c on f.creator_id=c.[user_id]
				left join dbo.[user] r on f.reviewer_id=r.[user_id]
				left join dbo.[user] a on f.approver_id=a.[user_id]
				left join dbo.[user] l on f.loader_id=l.[user_id]
				WHERE f.[status] = @status 
					  and (f.review_accepted is null or f.review_accepted=1)
					  and (f.approve_accepted is null or f.approve_accepted=1)
					  and (f.issuer_id = isnull(@issuer_id,f.issuer_id))
					  and (f.branch_id = isnull(@branch_id,f.branch_id))

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


