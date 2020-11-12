USE [indigo_database_group]
GO

--Create Enterprise Issuer
SET IDENTITY_INSERT [issuer] ON	
	INSERT INTO [dbo].[issuer]
           ([issuer_id], [issuer_status_id],[issuer_name],[issuer_code],[auto_create_dist_batch],[instant_card_issue_YN]
           ,[pin_mailer_printing_YN],[delete_pin_file_YN],[delete_card_file_YN],[license_file],[cards_file_location]
           ,[card_file_type],[pin_file_location],[pin_encrypted_ZPK],[pin_mailer_file_type],[pin_printer_name]
           ,[pin_encrypted_PWK], [language_id], [account_validation_YN], [maker_checker_YN], [country_id])
     VALUES (-1, 0, 'Enterprise', '', 0, 0, 0, 0, 0, '', '', '', '', '', 0, '', '', NULL, 0, 0, 2)
SET IDENTITY_INSERT [issuer] OFF

--Create default branch
SET IDENTITY_INSERT [branch] ON	
INSERT INTO [dbo].[branch] ([branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location]
           ,[contact_person],[contact_email],[card_centre])
     VALUES
           (-1, 0, -1, '', 'Enterprise Branch', '', '', '','')
SET IDENTITY_INSERT [branch] OFF	

--Create Defaul Groups
SET IDENTITY_INSERT [user_group] ON	
INSERT INTO [dbo].[user_group]
           ([user_group_id], [user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name])
     VALUES
           (-1, 8,-1, 1, 1, 1, 1, 1, 'GROUP_USER_GROUP_ADMIN')

INSERT INTO [dbo].[user_group]
           ([user_group_id], [user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name])
     VALUES
           (-2, 9,-1, 1, 1, 1, 1, 1, 'GROUP_USER_ADMIN')
SET IDENTITY_INSERT [user_group] OFF

--Create Default User
SET IDENTITY_INSERT [user] ON
	declare @objid int
	SET @objid = object_id('user')
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate	
		INSERT INTO [dbo].[user]
			   ([user_id],[user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[online]
			   ,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins],[last_password_changed_date]
			   ,[workstation],[language_id],[username_index],[user_email])
		 VALUES
			   (-1, 0, 3, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'WJy1rr/hG0Z7b4aFSiGmcA==')),
			   0, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
			   GETDATE(), GETDATE(), 0, GETDATE(), '', NULL, 
			   dbo.MAC('useradmin', @objid),'sys')
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
SET IDENTITY_INSERT [user] OFF
GO

INSERT INTO [dbo].[users_to_users_groups]
           ([user_id],[user_group_id])
     VALUES (-1, -1)
INSERT INTO [dbo].[users_to_users_groups]
           ([user_id],[user_group_id])
     VALUES (-1, -2)

GO

--Create System User
SET IDENTITY_INSERT [user] ON
	declare @objid int
	SET @objid = object_id('user')
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate	
		INSERT INTO [dbo].[user]
			   ([user_id],[user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[online]
			   ,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins],[last_password_changed_date]
			   ,[workstation],[language_id],[username_index],[user_email])
		 VALUES
			   (-2, 0, 3, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
			   ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'WJy1rr/hG0Z6b4aFSiGmcA==')),
			   0, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
			   GETDATE(), GETDATE(), 0, GETDATE(), '', NULL, 
			   dbo.MAC('SYSTEM', @objid), 'sys')
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
SET IDENTITY_INSERT [user] OFF
GO
