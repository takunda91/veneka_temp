-- =============================================
-- Author:		Richard Brenchley
-- Create date: 20 March 2014
-- Description:	Gets a user for login purposes.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_for_login] 
	-- Add the parameters for the stored procedure here
	@username varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--Update locked out user status if the locked out time has expired
		UPDATE [user]
		SET user_status_id = 0
			, number_of_incorrect_logins = 0
		WHERE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].[username])) = @username 
			AND [user].[user_status_id] = 3
			AND [user].[last_login_attempt] <= (SELECT TOP 1 DATEADD(HOUR, [user_admin].[PasswordAttemptLockoutDuration] * -1, GETDATE())
												FROM [user_admin])

		Declare @UserTimezone as nvarchar(50),@user_id as int

		SELECT TOP 1  @user_id= [user].[user_id]	
		FROM [user] 
		WHERE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].[username])) = @username

		set @UserTimezone=[dbo].[GetUserTimeZone](@user_id) 

		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
			,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].[username])) as 'clr_username'
			,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].[password])) as 'clr_password'
			,[user].[online]    
			,[user].[workstation]
			,cast(SWITCHOFFSET([user].last_login_attempt,@UserTimezone) as datetime) as 'last_login_attempt'
			,cast(SWITCHOFFSET([user].last_password_changed_date,@UserTimezone) as datetime) as'last_password_changed_date'
			,[user].number_of_incorrect_logins
			,[user].language_id
			,[user].user_status_id
			,[connection_parameters].address as domain_hostname_or_ip
			,[connection_parameters].domain_name
			,[connection_parameters].[path] as domain_path
			,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([connection_parameters].username)) as domain_username	
			,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([connection_parameters].[password])) as domain_password,
			[connection_parameters].connection_parameter_type_id, [connection_parameters].port,
			[connection_parameters].is_external_auth,[user].authentication_configuration_id,auth_type,protocol,[path]								
		FROM [user] 
		LEFT OUTER JOIN auth_configuration_connection_parameters
		 ON auth_configuration_connection_parameters.authentication_configuration_id= [user].authentication_configuration_id
			LEFT OUTER JOIN [connection_parameters]
				ON auth_configuration_connection_parameters.[connection_parameter_id] = [connection_parameters].[connection_parameter_id]
		WHERE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].[username])) = @username

		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END