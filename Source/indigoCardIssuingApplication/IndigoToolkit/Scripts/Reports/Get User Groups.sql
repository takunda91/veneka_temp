USE [DB_NAME]

SELECT [user_group_id]
	   ,[user_group_name]
	   ,[user_role_id]
       ,[can_create]
       ,[can_read]
       ,[can_update]
       ,[can_delete]
       ,[all_branch_access]
       
FROM [dbo].[user_group]
WHERE [issuer_id] = @selected_issuer_id