USE [DB_NAME]

EXEC [dbo].[sp_open_keys]


SELECT  DISTINCT [user].[user_id]
	       ,[dbo].[fn_decrypt_value]([user].[username], DEFAULT)   [username]
	       ,[dbo].[fn_decrypt_value]([user].[first_name], DEFAULT) [first_name]
	       ,[dbo].[fn_decrypt_value]([user].[last_name], DEFAULT)  [last_name]
	       ,[user_status].[user_status_text] [user_status_value]
	       ,[user_gender].[user_gender_text] [user_gender_value]
	       ,[user].[user_email]
	       ,[user].[last_login_date]
	       ,[user].[last_login_attempt]
	       ,[user].[workstation]
	
	FROM [dbo].[user] [user]
		LEFT JOIN [dbo].[users_to_users_groups] [user_group]
			ON [user].[user_id] = [user_group].[user_id]
		LEFT JOIN [dbo].[user_group] [group]
			ON [user_group].[user_group_id] = [group].[user_group_id]
		LEFT JOIN [dbo].[user_status]
			ON [user].[user_status_id] = [user_status].[user_status_id]
		LEFT JOIN [user_gender]
			ON [user].[user_gender_id] = [user_gender].[user_gender_id]
	WHERE [group].[issuer_id] = @selected_issuer_id


EXEC [dbo].[sp_close_keys]



