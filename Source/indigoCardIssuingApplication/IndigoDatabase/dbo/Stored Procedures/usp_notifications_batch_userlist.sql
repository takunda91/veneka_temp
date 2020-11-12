CREATE PROCEDURE [dbo].[usp_notifications_batch_userlist]
	@issuer_id int,
	@user_role_id int,
	@dist_batch_type_id int,
	@branch_id int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
	if(@dist_batch_type_id=0)
			BEGIN
		SELECT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].first_name)) as 'first_name', 
			   CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].last_name)) as 'last_name', 
			   [user].user_email
		FROM [user_group]
			INNER JOIN [users_to_users_groups]
				ON [users_to_users_groups].user_group_id = [user_group].user_group_id
					AND [user_group].user_role_id = @user_role_id
					AND  [user_group].issuer_id = @issuer_id
					AND [user_group].can_update = 1
					AND [user_group].can_create = 1
	
			INNER JOIN [user]
				ON [user].[user_id] = [users_to_users_groups].[user_id]
					AND [user].user_status_id = 0

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
		END
	ELSE 
	BEGIN
	SELECT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].first_name)) as 'first_name', 
			   CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].last_name)) as 'last_name', 
			   [user].user_email
		FROM [user_group]
			INNER JOIN [users_to_users_groups]
				ON [users_to_users_groups].user_group_id = [user_group].user_group_id
				INNER JOIN [user_groups_branches]
				ON [users_to_users_groups].user_group_id = [user_groups_branches].user_group_id
					AND [user_group].user_role_id = @user_role_id
					AND  [user_group].issuer_id = @issuer_id
					AND [user_group].can_update = 1
					AND [user_group].can_create = 1
					AND [user_groups_branches].branch_id=@branch_id
	
			INNER JOIN [user]
				ON [user].[user_id] = [users_to_users_groups].[user_id]
					AND [user].user_status_id = 0 
	END

END
