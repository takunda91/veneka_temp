[{DATABASE_NAME}].[dbo].[connection_parameters]


INSERT INTO [dbo].[connection_parameters]
   (connection_name, [address], port, [path], protocol, auth_type, username, [password], connection_parameter_type_id, domain_name,file_encryption_type_id)
SELECT  ldap_setting_name, hostname_or_ip, 0 as port, [path], 0 as protocol, 0 as auth_type, 
			[dbo].[fn_encrypt_value](COALESCE([{SOURCE_DATABASE_NAME}].[dbo].fn_decrypt_value([username], DEFAULT), '')), 
			[dbo].[fn_encrypt_value](COALESCE([{SOURCE_DATABASE_NAME}].[dbo].fn_decrypt_value([password], DEFAULT), '')), 
			4 as connection_parameter_type_id, 
			domain_name, 1 as file_encryption_type_id
FROM [{SOURCE_DATABASE_NAME}].[dbo].[ldap_setting]

EXEC [{SOURCE_DATABASE_NAME}].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]
GO

--- to upadte old data
DECLARE  @temp TABLE(ldap_setting_id int, connection_parameter_id int)

INSERT @temp(ldap_setting_id,connection_parameter_id)
SELECT ldap_setting_id,c.connection_parameter_id
FROM [{SOURCE_DATABASE_NAME}].[dbo].[ldap_setting]
	INNER JOIN [dbo].[connection_parameters] c
	ON [{SOURCE_DATABASE_NAME}].[dbo].[ldap_setting].ldap_setting_name = c.connection_name


UPDATE u 
SET connection_parameter_id = t.connection_parameter_id
FROM [dbo].[user] u INNER JOIN 
	@temp t on  t.ldap_setting_id = u.connection_parameter_id

--START BY EXPORTING LDAP SETTINGS
--SET IDENTITY_INSERT  [dbo].[ldap_setting] ON 

--INSERT INTO [dbo].[ldap_setting]
--           ([ldap_setting_id]
--		   ,[ldap_setting_name]
--           ,[hostname_or_ip]
--           ,[path]
--           ,[domain_name]
--           ,[username]
--           ,[password]
--		   ,[auth_type_id])
--SELECT [ldap_setting_id]
--      ,[ldap_setting_name]
--      ,[hostname_or_ip]
--      ,[path]
--      ,[domain_name]
--      ,[dbo].[fn_encrypt_value]([{SOURCE_DATABASE_NAME}].[dbo].fn_decrypt_value([username], DEFAULT))
--      ,[dbo].[fn_encrypt_value]([{SOURCE_DATABASE_NAME}].[dbo].fn_decrypt_value([password], DEFAULT))
--	  , 1
--  FROM [{SOURCE_DATABASE_NAME}].[dbo].[ldap_setting]
--  ORDER BY [ldap_setting_id] ASC


--SET IDENTITY_INSERT  [dbo].[ldap_setting] OFF 

