-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
Create  PROCEDURE [dbo].[usp_connection_parameter_update]
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
	@protocol int,
	@auth_type int,
	@header_length int = null,
	@identification varchar(100) = NULL,
	@timeout_milli int = NULL,
	@buffer_size int = NULL,
	@doc_type char = NULL,
	@auth_username VARCHAR(100) = NULL,
	@auth_password VARCHAR(100) = NULL,
	@auth_nonce VARCHAR(100)=NULL,
	@private_key VARCHAR(MAX) = NULL,
	@public_key VARCHAR(MAX) = NULL,
	@domain_name varchar(100)=null,
	@is_external_auth bit,
	@remote_port int = null,
	@remote_username VARCHAR(100) = NULL,
	@remote_password VARCHAR(100) = NULL,
	@additional_data AS bi_key_varchar_value_array readonly,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int output
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
					[nonce]=ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_nonce)),
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
					[is_external_auth]=@is_external_auth,
					[remote_port] = @remote_port,
					[remote_username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@remote_username)),
					[remote_password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@remote_password)) 
				WHERE [connection_parameter_id] = @connection_parameter_id


				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				INSERT  INTO [connection_parameters_additionaldata_audit] (connection_parameter_id, [key], value)
				select ad.connection_parameter_id ,ad.[key],ad.value
				from [connection_parameters_additionaldata] as ad
				where ad.connection_parameter_id= @connection_parameter_id

				Delete  from [connection_parameters_additionaldata] where connection_parameter_id= @connection_parameter_id 

				INSERT INTO [connection_parameters_additionaldata]
                         (connection_parameter_id, [key], value)
				Select @connection_parameter_id ,ad.key1,ad.value
				from @additional_data as ad


				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Parameter Updated: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))			
				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL		
				 set @ResultCode=0

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
