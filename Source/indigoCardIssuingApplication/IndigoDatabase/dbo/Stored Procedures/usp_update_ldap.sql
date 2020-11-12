-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/04/03
-- Description:	Updates an LDAP record
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_ldap]
	@ldap_setting_id int,
	@ldap_setting_name varchar(100),
	@hostname_or_ip varchar(200),	
	@path varchar(200),
	@domain_name varchar(100) = NULL,
	@auth_username varchar(100) = NULL,
	@auth_password varchar(100) = NULL,
	@auth_type_id int,
	@external_inteface_id char(36),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		--Check for duplicate's
	IF (SELECT COUNT(*) FROM [ldap_setting] WHERE [ldap_setting_name] = @ldap_setting_name AND [ldap_setting_id] != @ldap_setting_id) > 0
		BEGIN
			SET @ResultCode = 225						
		END
	ELSE
	BEGIN 
		BEGIN TRANSACTION [UPDATE_LDAP_TRAN]
		BEGIN TRY 

			IF @auth_username IS NULL
				SET @auth_username = ''

			IF @auth_password IS NULL
				SET @auth_password = ''

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				UPDATE [dbo].[ldap_setting]
				SET	[ldap_setting_name] = @ldap_setting_name,
					[hostname_or_ip] = @hostname_or_ip,
					[domain_name] = @domain_name,
					[path] = @path,
					[username] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_username)),
					[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@auth_password)),
					auth_type_id=@auth_type_id,
					external_inteface_id=@external_inteface_id
				WHERE ldap_setting_id = @ldap_setting_id


			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			--log the audit record
			DECLARE @audit_description varchar(max)
			SELECT @audit_description = 'LDAP Updated: ' + @ldap_setting_name
										+ ', Id: ' + CAST(@ldap_setting_id as varchar(max))		

			EXEC usp_insert_audit @audit_user_id, 
									9,
									NULL, 
									@audit_workstation, 
									@audit_description, 
									NULL, NULL, NULL, NULL	

			SET @ResultCode = 0

			COMMIT TRANSACTION [UPDATE_LDAP_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_LDAP_TRAN]
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