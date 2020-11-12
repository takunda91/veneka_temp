 -- =============================================
-- Author:		Sandhya konduru
-- Create date: 
-- Description:	Get a user based on the users system ID.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_user_pending_by_user_id] 
	-- Add the parameters for the stored procedure here
	@user_pending_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		Declare @UserTimezone as varchar(50)

		set @UserTimezone=[dbo].[GetUserTimeZone](@user_pending_id) 
		--There should only be one result
		SELECT TOP 1 [user_pending].[pending_user_id] as [user_id]			
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user_pending].[username])) as 'username'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user_pending].[first_name])) as 'first_name'
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user_pending].[last_name])) as 'last_name' 					
						,CONVERT(VARCHAR(max),DECRYPTBYKEY([user_pending].[employee_id])) as 'empoyee_id'
						,[user_pending].[user_email]
						,[user_pending].[user_status_id] 
						,[user_pending].authentication_configuration_id
						,[user_pending].[language_id]
						,[user_pending].[online]    
						,[user_pending].[workstation],
						
						(case when last_password_changed_date is null then 
						cast(SWITCHOFFSET( SYSDATETIMEOFFSET(),@UserTimezone) as datetime) 
						else cast(SWITCHOFFSET([user_pending].last_password_changed_date,@UserTimezone) as datetime) end ) as 'last_password_changed_date'
						,[user_pending].number_of_incorrect_logins,[user_pending].last_login_attempt,time_zone_id,time_zone_utcoffset
		FROM [user_pending]
		WHERE [user_pending].[pending_user_id] = @user_pending_id		

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END

