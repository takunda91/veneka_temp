


EXEC [indigo_database_group].[dbo].[sp_open_keys]
EXEC [dbo].[sp_open_keys]


SET IDENTITY_INSERT  [dbo].[user] ON

;WITH [decrypted] ([user_id]
      ,[user_status_id]
      ,[user_gender_id]
      ,[username]
      ,[first_name]
      ,[last_name]
      ,[password]
      ,[user_email]
      ,[online]
      ,[employee_id]
      ,[last_login_date]
      ,[last_login_attempt]
      ,[number_of_incorrect_logins]
      ,[last_password_changed_date]
      ,[workstation]
      ,[language_id]
      ,[username_index]
      ,authentication_configuration_id
      ,[instant_authorisation_pin]
      ,[last_authorisation_pin_changed_date])
AS
(
	SELECT  [user].[user_id]
	       ,[user].[user_status_id]
	       ,[user].[user_gender_id]
	       ,[indigo_database_group].[dbo].[fn_decrypt_value]([user].[username], DEFAULT)   [username]
	       ,[indigo_database_group].[dbo].[fn_decrypt_value]([user].[first_name], DEFAULT) [first_name]
	       ,[indigo_database_group].[dbo].[fn_decrypt_value]([user].[last_name], DEFAULT)  [last_name]
	       ,[indigo_database_group].[dbo].[fn_decrypt_value]([user].[password], DEFAULT)   [password]
	       ,[user].[user_email]
	       ,[user].[online]
	       ,[indigo_database_group].[dbo].fn_decrypt_value([user].[employee_id], DEFAULT) [employee_id]
	       ,[user].[last_login_date]
	       ,[user].[last_login_attempt]
	       ,[user].[number_of_incorrect_logins]
	       ,[user].[last_password_changed_date]
	       ,[user].[workstation]
	       ,[user].[language_id]
	       ,[user].[username_index]
	       ,NULL
		   ,NULL
		   ,NULL
	
	FROM [indigo_database_group].[dbo].[user] [user]
) 


--EXPORTING USERS

INSERT INTO [dbo].[user]
           ([user_id]
		   ,[user_status_id]
		   ,[user_gender_id]
		   ,[username]
		   ,[first_name]
		   ,[last_name]
		   ,[password]
		   ,[user_email]
		   ,[online]
		   ,[employee_id]
		   ,[last_login_date]
		   ,[last_login_attempt]
		   ,[number_of_incorrect_logins]
		   ,[last_password_changed_date]
		   ,[workstation]
		   ,[language_id]
		   ,[username_index]
		   ,[authentication_configuration_id]
		   ,[instant_authorisation_pin]
		   ,[last_authorisation_pin_changed_date]
		   )
SELECT  [user_id]
	   ,[user_status_id]
	   ,[user_gender_id]
	   ,[dbo].[fn_encrypt_value]([username]	 )
	   ,[dbo].[fn_encrypt_value]([first_name])
	   ,[dbo].[fn_encrypt_value]([last_name] )
	   ,[dbo].[fn_encrypt_value]([password]  )
	   ,[user_email]
	   ,[online]
	   ,[dbo].[fn_encrypt_value]([employee_id])
	   ,[last_login_date]
	   ,[last_login_attempt]
	   ,[number_of_incorrect_logins]
	   ,[last_password_changed_date]
	   ,[workstation]
	   ,[language_id]
	   ,[username_index]
	   ,NULL -- OR --> [ldap_setting_id]
	   ,NULL
	   ,NULL
	   ,NULL
FROM [decrypted]

SET IDENTITY_INSERT  [dbo].[user] OFF

EXEC [indigo_database_group].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]


INSERT INTO [dbo].[user_admin]
           ([PasswordValidPeriod]
           ,[PasswordMinLength]
           ,[PasswordMaxLength]
           ,[PreviousPasswordsCount]
           ,[maxInvalidPasswordAttempts]
           ,[PasswordAttemptLockoutDuration]
           ,[CreatedBy]
           ,[CreatedDateTime])
     VALUES
           (30	
           ,5	
           ,12	
           ,3	
           ,5	
           ,4
		   ,-1
           ,GETDATE())


---- MOVE THE SEED VALUE FOR SAFTY SAKE
--DECLARE @users_current_seed INT,
--		@users_new_seed INT

--SET @users_current_seed = 
--(
--	SELECT TOP 1 [user].[user_id]
--	FROM [dbo].[user]
--	ORDER BY [user].[user_id] DESC
--)

--SET @users_new_seed = (@users_current_seed * 1.5)


--DBCC CHECKIDENT('[dbo].[user]', RESEED, @users_new_seed);
