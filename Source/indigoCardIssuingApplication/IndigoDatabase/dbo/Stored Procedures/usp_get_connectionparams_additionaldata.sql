Create PROCEDURE [dbo].[usp_get_connectionparams_additionaldata]	
	@connection_parameter_id int
AS
BEGIN


	
SELECT        [key], value
FROM            [connection_parameters _additionaldata]
where connection_parameter_id=@connection_parameter_id

END