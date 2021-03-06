USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_issuers_for_role]    Script Date: 2015/04/24 11:43:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_all_issuers_for_role] 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
		   [issuer].maker_checker_YN, [issuer].account_validation_YN, 
		   [issuer].auto_create_dist_batch,
		   [issuer].classic_card_issue_YN, [issuer].instant_card_issue_YN, [issuer].card_ref_preference,
		   [issuer].enable_instant_pin_YN, [issuer].authorise_pin_issue_YN, [issuer].authorise_pin_reissue_YN,
		   [issuer].pin_mailer_printing_YN, [issuer].pin_mailer_reprint_YN,
		   [issuer].EnableCardFileLoader_YN
	FROM [issuer]
	WHERE issuer_status_id = 0
END






