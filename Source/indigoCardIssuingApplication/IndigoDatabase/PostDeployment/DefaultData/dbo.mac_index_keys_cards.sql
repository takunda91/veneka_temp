
DECLARE @cardsobjid int
SET @cardsobjid = object_id('cards')

-- Check if the card's table object id has changed.
IF NOT EXISTS (SELECT [table_id] FROM [dbo].[mac_index_keys] WHERE [table_id] = @cardsobjid)
BEGIN
	-- Adds new mac_key
	EXEC [dbo].[AddMacForTable]	@Table_id = @cardsobjid

	-- Check to see if we need to update cards index
	IF EXISTS (SELECT TOP 1 [card_id] FROM [dbo].[cards])
	BEGIN
		--DECLARE @objid INT
		--SET @objid = object_id('cards')			
		DECLARE @cards_key varbinary(100)
		--SET @key = null 

		-- Get the new mac key
		SELECT @cards_key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
		FROM [dbo].[mac_index_keys]
		WHERE table_id = @cardsobjid

		-- Update cards with new index
		UPDATE [dbo].[cards]
		SET	[card_index] = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT([card_number], 4))) + @cards_key ))
	END
END
