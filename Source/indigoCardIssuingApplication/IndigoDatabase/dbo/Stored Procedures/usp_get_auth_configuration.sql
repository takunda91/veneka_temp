CREATE PROCEDURE [dbo].[usp_get_auth_configuration]	
	@authentication_configuration_id int
AS
BEGIN


	
	SELECT        authentication_configuration_id, authentication_configuration
FROM            auth_configuration
where authentication_configuration_id=@authentication_configuration_id

END