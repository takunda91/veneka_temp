
CREATE PROCEDURE [dbo].[usp_insert_auth_configuration]
@authentication_configuration varchar(500),
@auth_configuration_interface as auth_configuration_interface readonly,
@audit_user_id bigint =null,
@audit_workstation varchar(100)=null,	
@authentication_configuration_id int output,
@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
		IF (SELECT COUNT(*) FROM [auth_configuration] WHERE ([authentication_configuration] = @authentication_configuration )) > 0
				BEGIN
					SET @authentication_configuration_id = 0
					SET @ResultCode = 210
				END
			ELSE
			BEGIN	
			
			BEGIN TRANSACTION [INSERT_AUTH_CONFIG_TRAN]
				BEGIN TRY 
		INSERT INTO auth_configuration
								 ( authentication_configuration)
		VALUES        (@authentication_configuration)

		set	@authentication_configuration_id = SCOPE_IDENTITY()
		
		INSERT INTO auth_configuration_connection_parameters
								 (authentication_configuration_id,auth_type_id,interface_guid,connection_parameter_id, external_interface_id)
		Select @authentication_configuration_id,auth_type_id,interface_guid,connection_parameter_id,external_interface_id
		from @auth_configuration_interface

					SET @ResultCode = 0

			COMMIT TRANSACTION [INSERT_AUTH_CONFIG_TRAN]			
		END TRY
	BEGIN CATCH		
		ROLLBACK TRANSACTION [INSERT_AUTH_CONFIG_TRAN]
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

END