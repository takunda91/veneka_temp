
DECLARE @userobjid int
SET @userobjid = object_id('user')

-- Check if the card's table object id has changed.
IF NOT EXISTS (SELECT [table_id] FROM [dbo].[mac_index_keys] WHERE [table_id] = @userobjid)
BEGIN
	-- Adds new mac_key
	EXEC [dbo].[AddMacForTable]	@Table_id = @userobjid

	-- Check to see if we need to update user index
	IF EXISTS (SELECT TOP 1 [user_id] FROM [dbo].[user])
	BEGIN		
		DECLARE @users_key varbinary(100)

		-- Get the new mac key
		SELECT @users_key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
		FROM [dbo].[mac_index_keys]
		WHERE table_id = @userobjid

		-- Update user records with new index
		UPDATE [dbo].[user]
		SET	[username_index] = CONVERT(varbinary(20),HashBytes( N'SHA1', CONVERT(varbinary(8000), [username]) + @users_key ))
	END
END