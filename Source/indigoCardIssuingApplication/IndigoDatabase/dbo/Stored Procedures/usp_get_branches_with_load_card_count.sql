-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetches all branches for a user as well as total available cards for distribution batch creation
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_with_load_card_count]
	@issuer_id int, 
	@user_id bigint,
	@user_role_id int,
	@load_card_status_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		--Get all branches belong to user_group where specific branches are listed
		SELECT [branch].branch_id, [branch].branch_name, [branch].branch_code, COUNT([load_batch_cards].card_id) AS CardCount
		FROM branch INNER JOIN user_groups_branches
				ON branch.branch_id = user_groups_branches.branch_id
			INNER JOIN user_group
				ON user_group.user_group_id = user_groups_branches.user_group_id
			INNER JOIN users_to_users_groups
				ON users_to_users_groups.user_group_id = user_group.user_group_id
			INNER JOIN [user]
				ON [user].[user_id] = users_to_users_groups.[user_id]
			LEFT OUTER JOIN [cards]
				ON [cards].branch_id = [branch].branch_id
			LEFT JOIN [load_batch_cards]
				ON [load_batch_cards].[card_id] = [cards].card_id
					AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
		WHERE user_group.issuer_id = @issuer_id
			AND user_group.user_role_id = @user_role_id
			AND [user].[user_id] = @user_id
			AND user_group.all_branch_access = 0
			AND branch.branch_status_id = 0
			AND [user].user_status_id = 0
		GROUP BY [branch].branch_id, [branch].branch_name, [branch].branch_code		
		UNION
		--Get all branches belong to user_group where all should be listed.
		SELECT [branch].branch_id, [branch].branch_name, [branch].branch_code, COUNT([load_batch_cards].card_id) AS CardCount
		FROM branch INNER JOIN issuer
				ON branch.issuer_id = issuer.issuer_id
			INNER JOIN user_group
				ON user_group.issuer_id = issuer.issuer_id
			INNER JOIN users_to_users_groups
				ON users_to_users_groups.user_group_id = user_group.user_group_id
			INNER JOIN [user]
				ON [user].[user_id] = users_to_users_groups.[user_id]
			LEFT OUTER JOIN [cards]
				ON [cards].branch_id = [branch].branch_id
			LEFT JOIN [load_batch_cards]
				ON [load_batch_cards].[card_id] = [cards].card_id
					AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
		WHERE user_group.issuer_id = @issuer_id
			AND user_group.user_role_id = @user_role_id
			AND [user].[user_id] = @user_id
			AND user_group.all_branch_access = 1
			AND branch.branch_status_id = 0
			AND [user].user_status_id = 0
		GROUP BY [branch].branch_id, [branch].branch_name, [branch].branch_code
END