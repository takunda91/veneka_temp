

/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_create]    Script Date: 2/22/2016 8:35:18 AM ******/
DROP PROCEDURE [dbo].[sp_connection_parameter_create]
GO

/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_create]    Script Date: 2/22/2016 8:35:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_connection_parameter_create] 
	@connection_name varchar(100),
	@connection_parameter_type_id int,
	@address varchar(200),
	@port int,
	@path varchar(200),
	@name_of_file varchar(100),
	@file_delete_YN bit,
	@file_encryption_type_id int,
	@duplicate_file_check_YN bit,
	@protocol varchar(50),
	@auth_type int,
	@header_length int = null,
	@identification varchar(100) = NULL,
	@timeout_milli int = NULL,
	@buffer_size int = NULL,
	@doc_type char = NULL,
	@auth_username VARCHAR(100),
	@auth_password VARCHAR(100),
	@private_key VARCHAR(MAX) = NULL,
	@public_key VARCHAR(MAX) = NULL,
	@domain_name varchar(100),
	@is_external_auth bit,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [INSERT_CANN_PARAM_TRAN]
		BEGIN TRY 

			DECLARE @connection_parameter_id int

			IF @identification IS NULL
				SET @identification = ''

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				INSERT INTO [dbo].[connection_parameters]
					   ([connection_name],[address],[port],[path],[protocol],[auth_type],[header_length],[identification],[username],[password],
						connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
						private_key, public_key,domain_name,is_external_auth )
				VALUES
					   (@connection_name, @address, @port, @path, @protocol, @auth_type, @header_length,
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@identification)),
					    ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),
						@connection_parameter_type_id,
						@timeout_milli,
						@buffer_size,
						@doc_type,
						@name_of_file,
						@file_delete_YN,
						@file_encryption_type_id,
						@duplicate_file_check_YN,
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@private_key)),
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@public_key)),@domain_name,@is_external_auth)		


				SET @connection_parameter_id = SCOPE_IDENTITY();

				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Parameter Created: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))

				EXEC sp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL		


				COMMIT TRANSACTION [INSERT_CANN_PARAM_TRAN]

				SELECT @connection_parameter_id

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_CANN_PARAM_TRAN]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH 	
END









GO


/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_update]    Script Date: 2/22/2016 8:40:46 AM ******/
DROP PROCEDURE [dbo].[sp_connection_parameter_update]
GO

/****** Object:  StoredProcedure [dbo].[sp_connection_parameter_update]    Script Date: 2/22/2016 8:40:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_connection_parameter_update]
	@connection_parameter_id int,
	@connection_name varchar(100),
	@connection_parameter_type_id int,
	@address varchar(200),
	@port int,
	@name_of_file varchar(100),
	@file_delete_YN bit,
	@file_encryption_type_id int,
	@duplicate_file_check_YN bit,
	@path varchar(200),
	@protocol varchar(50),
	@auth_type int,
	@header_length int = null,
	@identification varchar(100) = NULL,
	@timeout_milli int = NULL,
	@buffer_size int = NULL,
	@doc_type char = NULL,
	@auth_username VARCHAR(100) = NULL,
	@auth_password VARCHAR(100) = NULL,
	@private_key VARCHAR(MAX) = NULL,
	@public_key VARCHAR(MAX) = NULL,
	@domain_name varchar(100),
	@is_external_auth bit,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CONN_PARAM_TRAN]
		BEGIN TRY 

			IF @identification IS NULL
				SET @identification = ''

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			DECLARE @priv varbinary(max), @pub varbinary(max)

									

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				IF(ISNULL(@private_key, '') = '' )
					SELECT @priv = [private_key] FROM [connection_parameters] WHERE [connection_parameter_id] = @connection_parameter_id
				ELSE 
					SELECT @priv = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@private_key))


				IF(ISNULL(@public_key, '') = '' )
					SELECT @pub = [public_key] FROM [connection_parameters] WHERE [connection_parameter_id] = @connection_parameter_id
				ELSE 
					SELECT @pub = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@public_key))	

				UPDATE [dbo].[connection_parameters]
				SET [connection_name] = @connection_name,
					[address] = @address,
					[port] = @port,
					[path] = @path,
					[protocol] = @protocol,
					[auth_type] = @auth_type,
					[header_length] = @header_length,
					[identification] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@identification)),
					[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
					[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),
					[connection_parameter_type_id]=@connection_parameter_type_id,
					[timeout_milli] = @timeout_milli,
					[buffer_size] = @buffer_size,
					[doc_type] = @doc_type,
					[name_of_file] = @name_of_file,
					file_delete_YN = @file_delete_YN,
					file_encryption_type_id = @file_encryption_type_id,
					duplicate_file_check_YN = @duplicate_file_check_YN,
					[private_key] = @priv,
					[public_key] = @pub,
					[domain_name]=@domain_name,
					[is_external_auth]=@is_external_auth
				WHERE [connection_parameter_id] = @connection_parameter_id


				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Parameter Updated: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))			
				EXEC sp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL		


				COMMIT TRANSACTION [UPDATE_CONN_PARAM_TRAN]

		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_CONN_PARAM_TRAN]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH 	
END









GO




/****** Object:  StoredProcedure [dbo].[sp_get_connection_params]    Script Date: 2/22/2016 8:41:30 AM ******/
DROP PROCEDURE [dbo].[sp_get_connection_params]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_connection_params]    Script Date: 2/22/2016 8:41:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_connection_params] 
	-- Add the parameters for the stored procedure here
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, 
			file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key],domain_name ,is_external_auth
		FROM connection_parameters

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END









GO




/****** Object:  StoredProcedure [dbo].[sp_get_connection_parameter]    Script Date: 2/22/2016 8:41:19 AM ******/
DROP PROCEDURE [dbo].[sp_get_connection_parameter]
GO

/****** Object:  StoredProcedure [dbo].[sp_get_connection_parameter]    Script Date: 2/22/2016 8:41:19 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_connection_parameter] 
	-- Add the parameters for the stored procedure here
	@connection_parameter_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, 
			file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key],domain_name ,is_external_auth
		FROM connection_parameters
		WHERE connection_parameter_id = @connection_parameter_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END









GO




