USE [{DATABASE_NAME}]
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'P' AND name = 'sp_open_index_keys')
	DROP PROCEDURE [dbo].[sp_open_index_keys]
GO

CREATE PROCEDURE [dbo].[sp_open_index_keys]
AS
BEGIN
	BEGIN TRY

		OPEN SYMMETRIC KEY [key_Indexing]
		DECRYPTION BY CERTIFICATE [cert_ProtectIndexingKeys]

	END TRY
	BEGIN CATCH
		PRINT 'ERROR WHILE OPENING KEYS!!!'
	END CATCH
END
GO


IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'P' AND name = 'sp_close_index_keys')
	DROP PROCEDURE [dbo].[sp_close_index_keys]
GO

CREATE PROCEDURE [dbo].[sp_close_index_keys]
AS
BEGIN
	BEGIN TRY

		OPEN SYMMETRIC KEY [key_Indexing]
		DECRYPTION BY CERTIFICATE [cert_ProtectIndexingKeys]

	END TRY
	BEGIN CATCH
		PRINT 'ERROR WHILE CLOSING KEYS!!!'
	END CATCH
END
GO


IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'P' AND name = 'sp_close_keys')
	DROP PROCEDURE [dbo].[sp_close_keys]
GO

CREATE PROCEDURE [dbo].[sp_close_keys]
AS
BEGIN
	BEGIN TRY

		OPEN SYMMETRIC KEY [Indigo_Symmetric_Key]
		DECRYPTION BY CERTIFICATE [Indigo_Certificate]

	END TRY
	BEGIN CATCH
		PRINT 'ERROR WHILE CLOSING KEYS!!!'
	END CATCH
END
GO


IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'FN' AND name = 'fn_decrypt_value')
	DROP FUNCTION [dbo].[fn_decrypt_value]
GO

CREATE FUNCTION [dbo].[fn_decrypt_value] (@input VARBINARY(MAX), @certificate VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS BEGIN
	DECLARE @result VARCHAR(MAX)

	IF @certificate IS NULL
		SET @certificate = 'Indigo_Certificate'

	SET @result = CONVERT(VARCHAR, DECRYPTBYKEYAUTOCERT(CERT_ID(@certificate), NULL, (@input)))

	RETURN @result
END
GO


IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'FN' AND name = 'fn_encrypt_value')
	DROP FUNCTION [dbo].[fn_encrypt_value]
GO

CREATE FUNCTION [dbo].[fn_encrypt_value] (@input VARCHAR(MAX))
RETURNS VARBINARY(MAX)
AS BEGIN
	DECLARE @result VARBINARY(MAX)

	SET @result = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), @input)

	RETURN @result
END
GO



--USE [master]
--GO
--ALTER DATABASE [DB_NAME] SET  READ_WRITE 
--GO




USE [indigo_database_group] -- OLD DATABASE
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'P' AND name = 'sp_open_keys')
	DROP PROCEDURE 	[dbo].[sp_open_keys]
GO


CREATE PROCEDURE [dbo].[sp_open_keys]
AS
BEGIN
	BEGIN TRY

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate

	END TRY
	BEGIN CATCH
		PRINT 'ERROR WHILE OPENING KEYS!!!'
	END CATCH
END
GO


IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'P' AND name = 'sp_open_index_keys')
	DROP PROCEDURE 	[dbo].[sp_open_index_keys]
	GO


CREATE PROCEDURE [dbo].[sp_open_index_keys]
AS
BEGIN
	BEGIN TRY

		OPEN SYMMETRIC KEY [key_Indexing]
		DECRYPTION BY CERTIFICATE [cert_ProtectIndexingKeys]

	END TRY
	BEGIN CATCH
		PRINT 'ERROR WHILE OPENING KEYS!!!'
	END CATCH
END
GO


IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'P' AND name = 'sp_close_keys')
	DROP PROCEDURE 	[dbo].[sp_close_keys]
	GO



CREATE PROCEDURE [dbo].[sp_close_keys]
AS
BEGIN
	BEGIN TRY

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate

	END TRY
	BEGIN CATCH
		PRINT 'ERROR WHILE CLOSING KEYS!!!'
	END CATCH
END
GO


IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'P' AND name = 'sp_close_index_keys')
	DROP PROCEDURE 	[dbo].[sp_close_index_keys]
	GO



CREATE PROCEDURE [dbo].[sp_close_index_keys]
AS
BEGIN
	BEGIN TRY

		OPEN SYMMETRIC KEY [key_Indexing]
		DECRYPTION BY CERTIFICATE [cert_ProtectIndexingKeys]

	END TRY
	BEGIN CATCH
		PRINT 'ERROR WHILE CLOSING KEYS!!!'
	END CATCH
END
GO



IF EXISTS (SELECT * FROM SYSOBJECTS WHERE type = 'FN' AND name = 'fn_decrypt_value')
	DROP FUNCTION 	[dbo].[fn_decrypt_value]
	GO


CREATE FUNCTION [dbo].[fn_decrypt_value] (@input VARBINARY(MAX), @certificate VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS BEGIN
	DECLARE @result VARCHAR(MAX)

	IF @certificate IS NULL
		SET @certificate = 'Indigo_Certificate'

	SET @result = CONVERT(VARCHAR, DECRYPTBYKEYAUTOCERT(CERT_ID(@certificate), NULL, (@input)))

	RETURN @result
END
GO
