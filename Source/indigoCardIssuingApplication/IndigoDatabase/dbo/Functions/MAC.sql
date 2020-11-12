CREATE FUNCTION [dbo].[MAC]( @Message nvarchar(4000), @Table_id int )
RETURNS varbinary(24)
--WITH EXECUTE AS 'dbo'
AS
BEGIN
        declare @RetVal varbinary(24)
        declare @Key   varbinary(100)
        SET @RetVal = null
        SET @key    = null
        SELECT @Key = DecryptByKeyAutoCert( cert_id('cert_ProtectIndexingKeys'), null, mac_key) FROM mac_index_keys WHERE table_id = @Table_id
        if( @Key is not null )
               SELECT @RetVal = HashBytes( N'SHA1', convert(varbinary(8000), @Message) + @Key )
        RETURN @RetVal
END