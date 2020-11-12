-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/04/03
-- Description:	Creates a new LDAP record associated and returns the new record's ID
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_ldap]
	@ldap_setting_name varchar(100),
	@hostname_or_ip varchar(200),
	@path varchar(200),
	@domain_name varchar(100) = NULL,
	@auth_username varchar(200) = NULL,
	@auth_password varchar(200) = NULL,
	@auth_type_id int,
	@external_inteface_id char(36),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@new_ldap_id int OUTPUT,
	@ResultCode int OUTPUT
		
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Check for duplicate's
	IF (SELECT COUNT(*) FROM [ldap_setting] WHERE [ldap_setting_name] = @ldap_setting_name) > 0
		BEGIN
			SET @new_ldap_id = 0
			SET @ResultCode = 225						
		END
	ELSE
	BEGIN 

		BEGIN TRANSACTION [INSERT_LDAP_TRAN]
			BEGIN TRY 
				IF @auth_username IS NULL
					SET @auth_username = ''

				IF @auth_password IS NULL
					SET @auth_password = ''

				OPEN SYMMETRIC KEY Indigo_Symmetric_Key 
				DECRYPTION BY CERTIFICATE Indigo_Certificate

					INSERT INTO [dbo].[ldap_setting]
						([ldap_setting_name],[hostname_or_ip],[domain_name],[path],[username],[password],auth_type_id,external_inteface_id)
					VALUES
							(@ldap_setting_name, @hostname_or_ip, @domain_name, @path, 
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
							ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),@auth_type_id,@external_inteface_id)		


					SET @new_ldap_id = SCOPE_IDENTITY();

					CLOSE SYMMETRIC KEY Indigo_Symmetric_Key ;--Closes sym key

					--log the audit record
					DECLARE @audit_description varchar(max)
					SELECT @audit_description = 'LDAP Created: ' + @ldap_setting_name
												+ ', Id: ' + CAST(@new_ldap_id as varchar(max))

					EXEC usp_insert_audit @audit_user_id, 
											9,
											NULL, 
											@audit_workstation, 
											@audit_description, 
											NULL, NULL, NULL, NULL		

					SET @ResultCode = 0
					COMMIT TRANSACTION [INSERT_LDAP_TRAN]

			END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION [INSERT_LDAP_TRAN]
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
END