-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_passwordshistorystatus]
	@user_id int,
	@password varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			IF((SELECT COUNT(*) FROM  [user_password_history] WHERE 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([user_password_history].[password_history]))=@password and [user_password_history].[user_id] = @user_id)>0)
			set @ResultCode=0
			else 
			set @ResultCode=1

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
			END TRY
	BEGIN CATCH
		
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