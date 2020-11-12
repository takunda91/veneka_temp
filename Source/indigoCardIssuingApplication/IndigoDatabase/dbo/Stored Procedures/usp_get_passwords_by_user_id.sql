-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Returns decrypted passwords
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_passwords_by_user_id] 
	@user_id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		--For the current password add 99 days to show it's the latest. old passwords should be in the past.
		SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY([user].[password])) as 'password', DATEADD(DAY, 99, SYSDATETIMEOFFSET()) as 'date'						
		FROM [user]
		WHERE [user].[user_id] = @user_id
		UNION ALL
		SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY([user_password_history].[password_history])) as 'password', 
			   [user_password_history].[date_changed] as 'date'						
		FROM [user_password_history]
		WHERE [user_password_history].[user_id] = @user_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END