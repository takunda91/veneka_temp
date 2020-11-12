-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 22 April 2014
-- Description:	Updates a user's default language
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_user_language] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@language_id int,
	@result_code int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [UPDATE_USER_LANG_TRAN]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			UPDATE [dbo].[user]
			SET [language_id] = @language_id
			WHERE [user_id] = @user_id

			SET @result_code = '0'

			COMMIT TRANSACTION [UPDATE_USER_LANG_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_USER_LANG_TRAN]
		SET @result_code = 'An Exception Occurred.'
	  RETURN ERROR_MESSAGE()
	END CATCH 	

	RETURN
END