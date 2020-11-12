CREATE PROCEDURE [dbo].[usp_get_auth_configuration_connectionparams]	
	@authentication_configuration_id int
AS
BEGIN


	
SELECT        auth_configuration.authentication_configuration_id,auth_type_id,[interface_guid], auth_configuration_connection_parameters.connection_parameter_id, case when external_interface_id is null then ''  else external_interface_id end as 'external_interface_id'
,case when  connection_parameter_type_id is null then 0 else connection_parameter_type_id end as 'connection_parameter_type_id'
FROM            auth_configuration INNER JOIN
                         auth_configuration_connection_parameters ON auth_configuration.authentication_configuration_id = auth_configuration_connection_parameters.authentication_configuration_id
						Left JOIN  connection_parameters  ON auth_configuration_connection_parameters.connection_parameter_id = connection_parameters.connection_parameter_id
  
WHERE        (auth_configuration.authentication_configuration_id = @authentication_configuration_id)

END