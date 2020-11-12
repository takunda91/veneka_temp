CREATE PROC [dbo].[AddMacForTable] @Table_id int
--WITH EXECUTE AS 'dbo'
AS
        declare @Key    varbinary(100)
        declare @KeyGuid uniqueidentifier
        SET @KeyGuid = key_guid('key_Indexing')
        -- Open the encryption key
        -- Make sure the key is closed before doing any operation
		-- that may end the module, otherwise the key will
		-- remain opened after the store-procedure execution ends
        OPEN SYMMETRIC KEY key_Indexing DECRYPTION BY CERTIFICATE cert_ProtectIndexingKeys
 
        -- The new MAC key is derived from an encryption
		-- of a newly created GUID. As the encryption function
		-- is not deterministic, the output is random
        -- After getting this cipher, we calculate a SHA1 Hash for it.
        SELECT @Key = HashBytes( N'SHA1', ENCRYPTBYKEY( @KeyGuid, convert(varbinary(100), newid())) )
 
		-- Protect the new MAC key
        SET @KEY = ENCRYPTBYKEY( @KeyGuid, @Key )
 
        -- Closing the encryption key
        CLOSE SYMMETRIC KEY key_Indexing
        -- As we have closed the key we opened,
		-- it is safe to return from the SP at any time
 
        if @Key is null
        BEGIN
               RAISERROR( 'Failed to create new key.', 16, 1)
        END
        INSERT INTO mac_index_keys VALUES( @Table_id, @Key )