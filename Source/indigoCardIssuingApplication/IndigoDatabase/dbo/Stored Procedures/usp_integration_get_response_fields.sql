-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_get_response_fields] 
	-- Add the parameters for the stored procedure here
	@integration_name varchar(max),
	@integration_object_name varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DISTINCT [integration_fields].*
	FROM [integration]
			INNER JOIN [integration_object]
				ON [integration].integration_id = [integration_object].integration_id
			INNER JOIN [integration_fields]
				ON [integration_object].integration_id = [integration_fields].integration_id	
					AND [integration_object].integration_object_id = [integration_fields].integration_object_id
			INNER JOIN [integration_responses]
				ON [integration_fields].integration_id = [integration_responses].integration_id	
					AND [integration_fields].integration_object_id = [integration_responses].integration_object_id
					AND [integration_fields].integration_field_id = [integration_responses].integration_field_id
	WHERE UPPER([integration].integration_name) = UPPER(@integration_name)
			AND UPPER([integration_object].integration_object_name) = UPPER(@integration_object_name)
END