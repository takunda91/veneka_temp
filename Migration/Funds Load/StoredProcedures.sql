USE [indigo_database_2.1.4.0]
GO

/****** Object:  StoredProcedure [dbo].[usp_funds_load_create]    Script Date: 2019/11/06 16:04:23 ******/
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

/****** Object:  StoredProcedure [dbo].[usp_funds_load_review]    Script Date: 2019/11/06 16:04:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	review entry for funds_load
-- =============================================
CREATE PROCEDURE [dbo].[usp_funds_load_review] 
	-- Add the parameters for the stored procedure here
	@funds_load_id bigint,
	@reviewer_id	int,
	@review_date	datetime,
	@review_accepted bit, 
	@status int, 
	@ResultCode	int output
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_FUNDS_LOAD]
		BEGIN TRY 
		
			update funds_load
				set review_accepted=@review_accepted,
				review_date=@review_date,
				reviewer_id=@reviewer_id,
				[status]=@status
			where funds_load_id = @funds_load_id

			SET @ResultCode = 0			
	
			COMMIT TRANSACTION [UPDATE_FUNDS_LOAD]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_FUNDS_LOAD]
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

/****** Object:  StoredProcedure [dbo].[usp_funds_load_approve]    Script Date: 2019/11/06 16:04:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	approve entry for funds_load
-- =============================================
CREATE PROCEDURE [dbo].[usp_funds_load_approve] 
	-- Add the parameters for the stored procedure here
	@funds_load_id bigint,
	@approver_id	int,
	@approve_date	datetime,
	@approve_accepted bit, 
	@status int, 
	@ResultCode	int output
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_FUNDS_LOAD]
		BEGIN TRY 
		
			update funds_load
				set approve_accepted=@approve_accepted,
				approve_date=@approve_date,
				approver_id=@approver_id,
				[status]=@status
			where funds_load_id = @funds_load_id

			SET @ResultCode = 0			
	
			COMMIT TRANSACTION [UPDATE_FUNDS_LOAD]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_FUNDS_LOAD]
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

/****** Object:  StoredProcedure [dbo].[usp_funds_load_load]    Script Date: 2019/11/06 16:04:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	load entry for funds_load
-- =============================================
CREATE PROCEDURE [dbo].[usp_funds_load_load] 
	-- Add the parameters for the stored procedure here
	@funds_load_id bigint,
	@loader_id	int,
	@load_date	datetime,
	@status int, 
	@ResultCode	int output
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_FUNDS_LOAD]
		BEGIN TRY 
		
			update funds_load
				set load_date=@load_date,
				loader_id=@loader_id,
				[status]=@status
			where funds_load_id = @funds_load_id

			SET @ResultCode = 0			
	
			COMMIT TRANSACTION [UPDATE_FUNDS_LOAD]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_FUNDS_LOAD]
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

/****** Object:  StoredProcedure [dbo].[usp_funds_load_sms_send]    Script Date: 2019/11/06 16:04:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	sms entry for funds_load
-- =============================================
CREATE PROCEDURE [dbo].[usp_funds_load_sms_send]
	-- Add the parameters for the stored procedure here
	@funds_load_id bigint,
	@sms_sent_date	datetime,
	@status int, 
	@ResultCode	int output
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_FUNDS_LOAD]
		BEGIN TRY 
		
			update funds_load
				set sms_sent_date=@sms_sent_date,
				[status]=@status
			where funds_load_id = @funds_load_id

			SET @ResultCode = 0			
	
			COMMIT TRANSACTION [UPDATE_FUNDS_LOAD]
				
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_FUNDS_LOAD]
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

/****** Object:  StoredProcedure [dbo].[usp_funds_load_list]    Script Date: 2019/11/06 16:04:23 ******/
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
				FROM dbo.[funds_load] f
				inner join dbo.issuer i on f.issuer_id=i.issuer_id
				inner join dbo.branch b on f.branch_id=b.branch_id
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

/****** Object:  StoredProcedure [dbo].[usp_funds_load_list_prepaid_card]    Script Date: 2019/11/06 16:04:23 ******/
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

/****** Object:  StoredProcedure [dbo].[usp_funds_load_get]    Script Date: 2019/11/06 16:04:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Andrew Kudumba
-- Create date: 
-- Description:	Get card info
-- =============================================
CREATE  PROCEDURE [dbo].[usp_funds_load_get] 
	@funds_load_id bigint,
	@check_masking BIT,
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
				FROM dbo.[funds_load] f
				inner join dbo.issuer i on f.issuer_id=i.issuer_id
				inner join dbo.branch b on f.branch_id=b.branch_id
				WHERE f.funds_load_id = @funds_load_id
				

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


