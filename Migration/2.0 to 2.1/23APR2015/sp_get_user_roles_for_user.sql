USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_user_roles_for_user]    Script Date: 2015/04/24 11:48:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 11 March 2014
-- Description:	Get all the roles linked to the specified user.
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_user_roles_for_user]
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, [issuer].account_validation_YN, [issuer].auto_create_dist_batch,
			[issuer].classic_card_issue_YN, [issuer].instant_card_issue_YN, [issuer].card_ref_preference,
			[issuer].enable_instant_pin_YN, [issuer].authorise_pin_issue_YN, [issuer].authorise_pin_reissue_YN,
			[issuer].pin_mailer_printing_YN, [issuer].pin_mailer_reprint_YN, [issuer].EnableCardFileLoader_YN,
			[user_roles].allow_multiple_login, 
			[user_group].can_read, [user_group].can_create, [user_group].can_update
	FROM [user_group] 
			INNER JOIN [users_to_users_groups] 
				ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			INNER JOIN [user_roles]
				ON [user_group].user_role_id = [user_roles].user_role_id
			INNER JOIN [user] 
				ON [user].[user_id] = [users_to_users_groups].[user_id]			 
			INNER JOIN [issuer] 
				ON [user_group].issuer_id = [issuer].issuer_id					
	WHERE [user].[user_id] = @user_id
		  AND [issuer].issuer_status_id = 0
		  AND [user].user_status_id = 0
	GROUP BY [user_group].user_role_id, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
			[issuer].maker_checker_YN, [issuer].account_validation_YN, [issuer].auto_create_dist_batch,
			[issuer].classic_card_issue_YN, [issuer].instant_card_issue_YN, [issuer].card_ref_preference,
			[issuer].enable_instant_pin_YN, [issuer].authorise_pin_issue_YN, [issuer].authorise_pin_reissue_YN,
			[issuer].pin_mailer_printing_YN, [issuer].pin_mailer_reprint_YN, [issuer].EnableCardFileLoader_YN,
			[user_roles].allow_multiple_login, 
			[user_group].can_read, [user_group].can_create, [user_group].can_update
END







