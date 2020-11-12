
CREATE PROCEDURE [dbo].[usp_update_auth_configuration]
@authentication_configuration_id int,
@authentication_configuration varchar(500),
@auth_configuration_interface as auth_configuration_interface readonly,
@audit_user_id bigint =null,
@audit_workstation varchar(100)=null,
@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		BEGIN TRANSACTION [UPDATE_AUTH_CONFIG_TRAN]
				BEGIN TRY
		update auth_configuration set
		authentication_configuration=@authentication_configuration
		where authentication_configuration_id=@authentication_configuration_id
		
		

		INSERT INTO [auth_configuration_connection_parameters_audit]
								 (authentication_configuration_id,auth_type_id,[interface_guid], connection_parameter_id, external_interface_id)
		Select authentication_configuration_id,auth_type_id,[interface_guid],connection_parameter_id,external_interface_id
		from [dbo].[auth_configuration_connection_parameters]
		where authentication_configuration_id=@authentication_configuration_id

		Delete from auth_configuration_connection_parameters 
		where authentication_configuration_id=@authentication_configuration_id 

		INSERT INTO auth_configuration_connection_parameters
								 (authentication_configuration_id,auth_type_id,[interface_guid], connection_parameter_id, external_interface_id)
		Select @authentication_configuration_id,auth_type_id,interface_guid,connection_parameter_id,external_interface_id
		from @auth_configuration_interface

			set @ResultCode=0
		
		COMMIT TRANSACTION [UPDATE_AUTH_CONFIG_TRAN]			
		END TRY
	BEGIN CATCH		
		ROLLBACK TRANSACTION [UPDATE_AUTH_CONFIG_TRAN]
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