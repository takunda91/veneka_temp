
CREATE PROCEDURE [dbo].[usp_delete_authentication_configuration]
	@authentication_configuration_id int,	
	@audit_user_id bigint =null,
	@audit_workstation varchar(100)=null,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION [DELETE_AUTH_CONFIG_TRAN]
				BEGIN TRY
	DELETE FROM auth_configuration_connection_parameters   where authentication_configuration_id=@authentication_configuration_id

   DELETE FROM auth_configuration where authentication_configuration_id=@authentication_configuration_id

			SET @ResultCode = 0

			COMMIT TRANSACTION [DELETE_AUTH_CONFIG_TRAN]			
		END TRY
	BEGIN CATCH		
		ROLLBACK TRANSACTION [DELETE_AUTH_CONFIG_TRAN]
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