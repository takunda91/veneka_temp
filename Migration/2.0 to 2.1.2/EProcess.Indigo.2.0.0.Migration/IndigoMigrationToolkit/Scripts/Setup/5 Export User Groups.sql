

SET IDENTITY_INSERT  [dbo].[user_group] ON 

INSERT INTO [dbo].[user_group]
           ([user_group_id]
		   ,[user_role_id]
           ,[issuer_id]
           ,[can_create]
           ,[can_read]
           ,[can_update]
           ,[can_delete]
           ,[all_branch_access]
           ,[user_group_name]
           ,[mask_screen_pan]
           ,[mask_report_pan])
SELECT [user_group_id]
      ,[user_role_id]
      ,[issuer_id]
      ,[can_create]
      ,[can_read]
      ,[can_update]
      ,[can_delete]
      ,[all_branch_access]
      ,[user_group_name]
	  ,0
	  ,0
  FROM [indigo_database_group].[dbo].[user_group]
  ORDER BY [user_group_id] ASC

SET IDENTITY_INSERT  [dbo].[user_group] OFF 


INSERT INTO [dbo].[user_groups_branches]
           ([user_group_id]
           ,[branch_id])
	SELECT [user_group_for_branch].[user_group_id]
	      ,[user_group_for_branch].[branch_id]
	  FROM [indigo_database_group].[dbo].[user_groups_branches] [user_group_for_branch]
	      LEFT JOIN [indigo_database_group].[dbo].[branch] [branch]
			ON [user_group_for_branch].[branch_id] = [branch].[branch_id]
	  ORDER BY [user_group_id] ASC



INSERT INTO [dbo].[users_to_users_groups]
           ([user_id]
           ,[user_group_id])
	SELECT [user_groups].[user_id]
		  ,[user_groups].[user_group_id]
	  FROM [indigo_database_group].[dbo].[users_to_users_groups] [user_groups]
	      LEFT JOIN [indigo_database_group].[dbo].[user_group] [groups]
			ON [user_groups].[user_group_id] = [groups].[user_group_id]
	  ORDER BY [user_id] ASC


EXEC [indigo_database_group].[dbo].[sp_open_keys]
EXEC [dbo].[sp_open_keys]

INSERT INTO [dbo].[user_password_history]
           ([user_id]
           ,[password_history]
           ,[date_changed])
	SELECT [password_history].[user_id]
		  ,[dbo].[fn_encrypt_value]([indigo_database_group].[dbo].[fn_decrypt_value]([password_history].[password_history], DEFAULT))
		  ,[password_history].[date_changed]
	  FROM [indigo_database_group].[dbo].[user_password_history] [password_history]
	      LEFT JOIN [indigo_database_group].[dbo].[user] [user]
		      ON [password_history].[user_id] = [user].[user_id]
		  LEFT JOIN [indigo_database_group].[dbo].[users_to_users_groups] [user_group]
		      ON [user].[user_id] = [user_group].[user_id]
		  LEFT JOIN [indigo_database_group].[dbo].[user_group] [group]
		      ON [user_group].[user_group_id] = [group].[user_group_id]
	  ORDER BY [user_id] ASC

EXEC [indigo_database_group].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]


---- MOVE THE SEED VALUE FOR SAFTY SAKE
--DECLARE @user_group_current_seed INT,
--		@user_group_new_seed INT

--SET @user_group_current_seed = 
--(
--	SELECT TOP 1 [user_group].[user_group_id]
--	FROM [dbo].[user_group]
--	ORDER BY [user_group].[user_group_id] DESC
--)

--SET @user_group_new_seed = (@user_group_current_seed * 1.5)


--DBCC CHECKIDENT('[dbo].[user_group]', RESEED, @user_group_new_seed);