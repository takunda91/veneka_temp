

EXEC [indigo_database_group].[dbo].[sp_open_keys]
EXEC [dbo].[sp_open_keys]

--START BY EXPORTING LDAP SETTINGS
SET IDENTITY_INSERT  [dbo].[ldap_setting] ON 

INSERT INTO [dbo].[ldap_setting]
           ([ldap_setting_id]
		   ,[ldap_setting_name]
           ,[hostname_or_ip]
           ,[path]
           ,[domain_name]
           ,[username]
           ,[password])
SELECT [ldap_setting_id]
      ,[ldap_setting_name]
      ,[hostname_or_ip]
      ,[path]
      ,[domain_name]
      ,[dbo].[fn_encrypt_value]([indigo_database_group].[dbo].fn_decrypt_value([username], DEFAULT))
      ,[dbo].[fn_encrypt_value]([indigo_database_group].[dbo].fn_decrypt_value([password], DEFAULT))
  FROM [indigo_database_group].[dbo].[ldap_setting]
  ORDER BY [ldap_setting_id] ASC


SET IDENTITY_INSERT  [dbo].[ldap_setting] OFF 

