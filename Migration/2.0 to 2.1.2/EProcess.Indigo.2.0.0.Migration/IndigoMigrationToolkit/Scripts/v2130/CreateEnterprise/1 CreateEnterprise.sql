USE [{DATABASE_NAME}]
GO

SET IDENTITY_INSERT [issuer] ON	

INSERT INTO [dbo].[issuer]
           ([issuer_id]
		   ,[issuer_status_id]
           ,[country_id]
           ,[issuer_name]
           ,[issuer_code]
           ,[instant_card_issue_YN]
           ,[maker_checker_YN]
           ,[license_file]
           ,[license_key]
           ,[language_id]
           ,[card_ref_preference]
           ,[classic_card_issue_YN]
           ,[enable_instant_pin_YN]
           ,[authorise_pin_issue_YN]
           ,[authorise_pin_reissue_YN]
           ,[back_office_pin_auth_YN])
VALUES ( -1, 0, 0, 'Enterprise', '', 0, 0, '', '', 0, 0, 0, 0, 0, 0, 0 )

SET IDENTITY_INSERT [issuer] OFF
GO

SET IDENTITY_INSERT [branch] ON	
INSERT INTO [dbo].[branch] ([branch_id],[branch_status_id],[issuer_id],[branch_code],[branch_name],[location]
           ,[contact_person],[contact_email],[card_centre], [card_centre_branch_YN])
     VALUES
           (-1, 0, -1, '', 'Enterprise Branch', '', '', '','', 0)
SET IDENTITY_INSERT [branch] OFF	
GO

SET IDENTITY_INSERT [user_group] ON	
INSERT INTO [dbo].[user_group]
           ([user_group_id], [user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name],[mask_report_pan],[mask_screen_pan])
     VALUES
           (-1, 8,-1, 1, 1, 1, 1, 1, 'GROUP_USER_GROUP_ADMIN', 1, 1)

INSERT INTO [dbo].[user_group]
           ([user_group_id], [user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name],[mask_report_pan],[mask_screen_pan])
     VALUES
           (-2, 9,-1, 1, 1, 1, 1, 1, 'GROUP_USER_ADMIN', 1, 1)
SET IDENTITY_INSERT [user_group] OFF
GO

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