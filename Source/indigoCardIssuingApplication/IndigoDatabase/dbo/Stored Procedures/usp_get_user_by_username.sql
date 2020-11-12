-- =============================================
-- Author:		Richard Brenchley
-- Create date: 14 March 2014
-- Description:	Return a user based on the username
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_by_username] 
	-- Add the parameters for the stored procedure here
	@username varchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		Declare @UserTimezone as varchar(50)
		Declare @user_id as bigint
		SELECT TOP 1  @user_id= [user].[user_id] FROM [user] WHERE [user].[user_id] = @user_id

		set @UserTimezone=[dbo].[GetUserTimeZone](@user_id) 
		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_email]
						,[user].[language_id]
						,[user].[user_status_id] 
						,[user].[online]    
						,[user].[workstation],
						(case when last_password_changed_date is null then 
						cast(SWITCHOFFSET( SYSDATETIMEOFFSET(),@UserTimezone) as datetime) 
						else cast(SWITCHOFFSET([user].last_password_changed_date,@UserTimezone) as datetime) end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt,[user].authentication_configuration_id,time_zone_id,time_zone_utcoffset

		FROM [user]
		WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) = @username		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END