-- =============================================
-- Author:		Richard Brenchley
-- Create date: 11 March 2014
-- Description:	Get all the roles linked to the specified user.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_roles_for_user]
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, 
			[issuer].classic_card_issue_YN, [issuer].instant_card_issue_YN, [issuer].card_ref_preference,
			[issuer].enable_instant_pin_YN, [issuer].authorise_pin_issue_YN, [issuer].authorise_pin_reissue_YN,			
			[issuer].back_office_pin_auth_YN,
		case when[issuer].allow_branches_to_search_cards is null then cast( 0 as bit) else [issuer].allow_branches_to_search_cards end allow_branches_to_search_cards,
			[user_roles].allow_multiple_login, 
			[user_group].can_read, [user_group].can_create, [user_group].can_update,
				CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].account_validation_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 1 ELSE 0 END as BIT) AS account_validation_YN,
			CAST(CASE WHEN 1 = ALL (SELECT [issuer_product].auto_approve_batch_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 1 ELSE 0 END as BIT) AS auto_create_dist_batch
						
			,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_printing_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_printing_YN
			,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].pin_mailer_reprint_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 1 ELSE 0 END as BIT) AS pin_mailer_reprint_YN

			,CAST(CASE WHEN 0 = ANY (SELECT [issuer_product].enable_instant_pin_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 1 ELSE 0 END as BIT) AS enable__product_instant_pin_YN
			,CAST(CASE WHEN 1 = ANY (SELECT [issuer_product].enable_instant_pin_reissue_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 0 ELSE 1 END as BIT) AS enable_instant_pin_reissue_YN

			,CAST(CASE WHEN 0 = ANY (SELECT [issuer_product].cms_exportable_YN FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 1 ELSE 0 END as BIT) AS cms_exportable_YN

			,CAST(CASE WHEN 0 <> ANY (SELECT [issuer_product].product_load_type_id FROM [issuer_product] WHERE [issuer_product].issuer_id = [issuer].issuer_id AND [issuer_product].DeletedYN = 0)
				THEN 1 ELSE 0 END as BIT) AS EnableCardFileLoader_YN

			,CAST(CASE WHEN 2 = ANY (SELECT b.branch_type_id FROM [branch] as b WHERE b.issuer_id = [issuer].issuer_id and b.branch_id=[user_groups_branches].branch_id AND b.branch_status_id = 0)
OR ([user_group].all_branch_access=1 and 2 = ANY (SELECT b.branch_type_id FROM [branch] as b WHERE b.issuer_id = [issuer].issuer_id AND b.branch_status_id = 0)  )
			THEN 1 ELSE 0 END as BIT) AS SatelliteBranch_YN

			,CAST(CASE WHEN 1 = ANY (SELECT b.branch_type_id FROM [branch] as b WHERE b.issuer_id = [issuer].issuer_id and b.branch_id=[user_groups_branches].branch_id AND b.branch_status_id = 0  )
 OR ([user_group].all_branch_access=1 and 1 = ANY (SELECT b.branch_type_id FROM [branch] as b WHERE b.issuer_id = [issuer].issuer_id AND b.branch_status_id = 0)  )
				THEN 1 ELSE 0 END as BIT) AS MainBranch_YN

	FROM [user_group] 
			INNER JOIN [users_to_users_groups] 
				ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			INNER JOIN [user_roles]
				ON [user_group].user_role_id = [user_roles].user_role_id
			INNER JOIN [user] 
				ON [user].[user_id] = [users_to_users_groups].[user_id]			 
			INNER JOIN [issuer] 
				ON [user_group].issuer_id = [issuer].issuer_id
		Left JOIN user_groups_branches 
				ON [user_group].user_group_id = user_groups_branches.user_group_id
			Left join [dbo].[branch] 
				ON user_groups_branches.branch_id = [branch].branch_id

	WHERE [user].[user_id] = @user_id
		  AND [issuer].issuer_status_id = 0
		  AND [user].user_status_id = 0
	GROUP BY [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, 
			[issuer].classic_card_issue_YN, [issuer].instant_card_issue_YN, [issuer].card_ref_preference,
			[issuer].enable_instant_pin_YN, [issuer].authorise_pin_issue_YN, [issuer].authorise_pin_reissue_YN,
			[issuer].back_office_pin_auth_YN, [user_roles].allow_multiple_login, user_groups_branches.branch_id,
			[user_group].can_read, [user_group].can_create, [user_group].can_update,allow_branches_to_search_cards,
			[user_group].all_branch_access
END