SET IDENTITY_INSERT [dbo].[user] ON;

DECLARE @objid int
SET @objid = object_id('user')
OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate

-- Insert initial useradmin
IF NOT EXISTS (SELECT [user_id] FROM [dbo].[user] WHERE [user_id] = -1)
BEGIN
	INSERT INTO [dbo].[user]
		([user_id],[user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[online]
		,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins],[last_password_changed_date]
		,[workstation],[language_id],[username_index],[user_email],[authentication_configuration_id], [time_zone_utcoffset],[time_zone_id])
	VALUES
		(-1, 0, 3, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'WJy1rr/hG0Z7b4aFSiGmcA==')),
		0, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useradmin')),
		SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET(), '', 0, 
		dbo.MAC('useradmin', @objid),'sys',1,'+00:00','GMT Standard Time')
END

-- Insert initial SYSTEM
IF NOT EXISTS (SELECT [user_id] FROM [dbo].[user] WHERE [user_id] = -2)
BEGIN
	INSERT INTO [dbo].[user]
		([user_id],[user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[online]
		,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins],[last_password_changed_date]
		,[workstation],[language_id],[username_index],[user_email],[authentication_configuration_id],
		[time_zone_utcoffset],[time_zone_id])
	VALUES
		(-2, 0, 3, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'WJy1rr/hG0Z6b4aFSiGmcA==')),
		0, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'SYSTEM')),
		SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET(), '', 0, 
		dbo.MAC('SYSTEM', @objid), 'sys',1,'+00:00','GMT Standard Time')
END

-- Insert initial useraudit
IF NOT EXISTS (SELECT [user_id] FROM [dbo].[user] WHERE [user_id] = -3)
BEGIN
	INSERT INTO [dbo].[user]
		([user_id],[user_status_id],[user_gender_id],[username],[first_name],[last_name],[password],[online]
		,[employee_id],[last_login_date],[last_login_attempt],[number_of_incorrect_logins],[last_password_changed_date]
		,[workstation],[language_id],[username_index],[user_email],[authentication_configuration_id],
		[time_zone_utcoffset],[time_zone_id])
	VALUES
		(-3, 0, 3, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
		ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'WJy1rr/hG0Z7b4aFSiGmcA==')),
		0, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'useraudit')),
		SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET(), '', 0, 
		dbo.MAC('useraudit', @objid),'sys',1,'+00:00','GMT Standard Time')
END

UPDATE [dbo].[user] SET [time_zone_utcoffset]='+00:00',[time_zone_id]='GMT Standard Time'

CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

SET IDENTITY_INSERT [dbo].[user] OFF;
