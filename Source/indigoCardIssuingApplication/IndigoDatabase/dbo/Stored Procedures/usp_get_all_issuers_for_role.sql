-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_all_issuers_for_role] 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT 
		[issuer].issuer_id
		, [issuer].issuer_name
		, [issuer].issuer_code
		, [issuer].maker_checker_YN		
		, [issuer].classic_card_issue_YN
		, [issuer].instant_card_issue_YN
		, [issuer].card_ref_preference
		, [issuer].enable_instant_pin_YN
		, [issuer].authorise_pin_issue_YN
		, [issuer].authorise_pin_reissue_YN		
		, [issuer].back_office_pin_auth_YN
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].account_validation_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS account_validation_YN
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].auto_approve_batch_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS auto_create_dist_batch
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_printing_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_printing_YN
		,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_reprint_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_reprint_YN
		,CAST(CASE WHEN 0 <> ANY (SELECT [issuer_product].product_load_type_id FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id)
				THEN 1 ELSE 0 END as BIT) AS EnableCardFileLoader_YN

	FROM [issuer]
	WHERE issuer_status_id = 0
END