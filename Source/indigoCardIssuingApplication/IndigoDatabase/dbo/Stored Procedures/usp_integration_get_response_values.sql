-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_get_response_values] 
	-- Add the parameters for the stored procedure here
	@field_list AS dbo.trikey_value_array READONLY,
	@language_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [integration_responses].*, [integration_responses_language].response_text
	FROM [integration_responses]
			LEFT OUTER JOIN [integration_responses_language]
				ON [integration_responses].integration_id = [integration_responses_language].integration_id
					AND [integration_responses].integration_object_id = [integration_responses_language].integration_object_id
					AND [integration_responses].integration_field_id = [integration_responses_language].integration_field_id
					AND [integration_responses].integration_response_id = [integration_responses_language].integration_response_id
					AND [integration_responses_language].language_id = @language_id
			INNER JOIN @field_list fields
				ON [integration_responses].integration_id = fields.key1
					AND [integration_responses].integration_object_id = fields.key2
					AND [integration_responses].integration_field_id = fields.key3
					AND fields.value LIKE [integration_responses].integration_response_value
	 
END