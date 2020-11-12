-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_connection_parameter_create] 
	@connection_name varchar(100),
	@connection_parameter_type_id int,
	@address varchar(200),
	@port int,
	@path varchar(200),
	@name_of_file varchar(100),
	@file_delete_YN bit,
	@file_encryption_type_id int,
	@duplicate_file_check_YN bit,
	@protocol int,
	@auth_type int,
	@header_length int = null,
	@identification varchar(100) = NULL,
	@additional_data AS bi_key_varchar_value_array readonly,
	@timeout_milli int = NULL,
	@buffer_size int = NULL,
	@doc_type char = NULL,
	@auth_username VARCHAR(100),
	@auth_password VARCHAR(100),
	@auth_nonce VARCHAR(100),
	@private_key VARCHAR(MAX) = NULL,
	@public_key VARCHAR(MAX) = NULL,
	@domain_name varchar(100)=NULL,
	@is_external_auth bit,
	@remote_port int = null,
	@remote_username VARCHAR(100) = NULL,
	@remote_password VARCHAR(100) = NULL,
	
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@connection_parameter_id int OUTPUT,
	@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [INSERT_CANN_PARAM_TRAN]
		BEGIN TRY 

			--DECLARE @connection_parameter_id int

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
						private_key, public_key,domain_name,is_external_auth, remote_port, remote_username, remote_password,nonce)
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
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@public_key)),@domain_name,@is_external_auth,
						@remote_port, 
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@remote_username)), 
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@remote_password)),
						ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_nonce)))		


				SET @connection_parameter_id = SCOPE_IDENTITY();
				
				INSERT INTO [connection_parameters_additionaldata]
                         (connection_parameter_id, [key], value)
				Select @connection_parameter_id ,ad.key1,ad.value
				from @additional_data as ad



				CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

				set @ResultCode=0
				--log the audit record
				DECLARE @audit_description varchar(max)
				SELECT @audit_description = 'Parameter Created: ' + @connection_name
											+ ' , Paramter Id: ' + CAST(@connection_parameter_id AS varchar(max))

				EXEC usp_insert_audit @audit_user_id, 
									 9,
									 NULL, 
									 @audit_workstation, 
									 @audit_description, 
									 NULL, NULL, NULL, NULL		


				COMMIT TRANSACTION [INSERT_CANN_PARAM_TRAN]


		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [INSERT_CANN_PARAM_TRAN]				 
		set @connection_parameter_id=0
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