
-- Only need initial value inserted
IF NOT EXISTS (SELECT [fileloader_status] FROM [dbo].[fileloader_status])
BEGIN
    INSERT INTO [dbo].[fileloader_status] VALUES(0, SYSDATETIMEOFFSET())
END
