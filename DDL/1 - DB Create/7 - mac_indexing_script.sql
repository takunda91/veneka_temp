USE [indigo_database_group]
GO

--Create Master Key
CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Jungle01$'
GO

--Create General encryption key
CREATE CERTIFICATE Indigo_Certificate WITH SUBJECT = 'Data protection'
GO

CREATE SYMMETRIC KEY Indigo_Symmetric_Key WITH ALGORITHM = AES_256 
ENCRYPTION BY CERTIFICATE Indigo_Certificate
GO

-- This is the certificate that will protect our MAC key-encryption key
CREATE CERTIFICATE cert_ProtectIndexingKeys WITH SUBJECT = 'Data indexing key protection'
GO 
-- This key will be used to protect the MAC keys
CREATE SYMMETRIC KEY key_Indexing WITH ALGORITHM = AES_256
ENCRYPTION BY CERTIFICATE cert_ProtectIndexingKeys
GO

--CREATE FUNCTION MAC( @Message nvarchar(4000), @Table_id int )
--RETURNS varbinary(24)
----WITH EXECUTE AS 'dbo'
--AS
--BEGIN
--        declare @RetVal varbinary(24)
--        declare @Key   varbinary(100)
--        SET @RetVal = null
--        SET @key    = null
--        SELECT @Key = DecryptByKeyAutoCert( cert_id('cert_ProtectIndexingKeys'), null, mac_key) FROM mac_index_keys WHERE table_id = @Table_id
--        if( @Key is not null )
--               SELECT @RetVal = HashBytes( N'SHA1', convert(varbinary(8000), @Message) + @Key )
--        RETURN @RetVal
--END
--GO

--CREATE PROC AddMacForTable @Table_id int
--WITH EXECUTE AS 'dbo'
--AS
--        declare @Key    varbinary(100)
--        declare @KeyGuid uniqueidentifier
--        SET @KeyGuid = key_guid('key_Indexing')
--        -- Open the encryption key
--        -- Make sure the key is closed before doing any operation
--		-- that may end the module, otherwise the key will
--		-- remain opened after the store-procedure execution ends
--        OPEN SYMMETRIC KEY key_Indexing DECRYPTION BY CERTIFICATE cert_ProtectIndexingKeys
 
--        -- The new MAC key is derived from an encryption
--		-- of a newly created GUID. As the encryption function
--		-- is not deterministic, the output is random
--        -- After getting this cipher, we calculate a SHA1 Hash for it.
--        SELECT @Key = HashBytes( N'SHA1', ENCRYPTBYKEY( @KeyGuid, convert(varbinary(100), newid())) )
 
--		-- Protect the new MAC key
--        SET @KEY = ENCRYPTBYKEY( @KeyGuid, @Key )
 
--        -- Closing the encryption key
--        CLOSE SYMMETRIC KEY key_Indexing
--        -- As we have closed the key we opened,
--		-- it is safe to return from the SP at any time
 
--        if @Key is null
--        BEGIN
--               RAISERROR( 'Failed to create new key.', 16, 1)
--        END
--        INSERT INTO mac_index_keys VALUES( @Table_id, @Key )
--GO


declare @objid int
SET @objid = object_id('cards')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO

declare @objid int
SET @objid = object_id('user')
EXEC	[dbo].[AddMacForTable]
		@Table_id = @objid
GO