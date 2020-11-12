CREATE PROCEDURE [dbo].[usp_get_user]
	(
		@username varchar(256),
		@user_status varchar(30)
	)
AS
	/* SET NOCOUNT ON */

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
	Declare @UserTimezone as varchar(50)
		Declare @user_id as bigint
		SELECT TOP 1  @user_id= [user].[user_id] FROM [user] WHERE [user].[user_id] = @user_id

		set @UserTimezone=[dbo].[GetUserTimeZone](@user_id) 

    SELECT user_id, u.user_status_id, user_gender_id, username, first_name,
		   last_name, CONVERT(VARCHAR(max),DECRYPTBYKEY(password)), online, employee_id, last_login_date,
		  cast(SWITCHOFFSET(u.last_login_attempt,@UserTimezone) as datetime) as last_login_attempt , number_of_incorrect_logins,  cast(SWITCHOFFSET(u.last_password_changed_date,@UserTimezone) as datetime) ,
		   workstation ,time_zone_id,time_zone_utcoffset,authentication_configuration_id
	FROM [user] u,user_status us
	WHERE (DECRYPTBYKEY(username)=@username And us.user_status_text = @user_status)
	
CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;