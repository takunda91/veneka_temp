-- =============================================
-- Author:		Richard Brenchley
-- Create date: 24 March 2014
-- Description:	Get a user based on the users system ID.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_by_user_id] 
	-- Add the parameters for the stored procedure here
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		Declare @UserTimezone as varchar(50)

		set @UserTimezone=[dbo].[GetUserTimeZone](@user_id) 
		--There should only be one result
		SELECT TOP 1 [user].[user_id]						
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[employee_id])) as 'empoyee_id'
						,[user].[user_email]
						,[user].[user_status_id] 
						,[user].[language_id]
						,[user].[online]    
						,[user].[workstation],
						
						(case when last_password_changed_date is null then 
						cast(SWITCHOFFSET( SYSDATETIMEOFFSET(),@UserTimezone) as datetime) 
						else cast(SWITCHOFFSET([user].last_password_changed_date,@UserTimezone) as datetime) end ) as 'last_password_changed_date'
						,[user].number_of_incorrect_logins,[user].last_login_attempt,[user].authentication_configuration_id,time_zone_id,time_zone_utcoffset
		FROM [user]
		WHERE [user].[user_id] = @user_id		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END