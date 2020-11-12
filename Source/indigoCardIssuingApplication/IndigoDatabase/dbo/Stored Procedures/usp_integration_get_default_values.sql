-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_get_default_values] 
	-- Add the parameters for the stored procedure here
	@integration_name varchar(max),
	@integration_object_name varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT DISTINCT [integration_fields].integration_id,
						[integration_fields].integration_object_id,
						[integration_fields].integration_field_id,
						[integration_fields].integration_field_name, 
						CONVERT(VARCHAR(max),DECRYPTBYKEY([integration_fields].integration_field_default_value)) as integration_field_default_value
		FROM [integration]
				INNER JOIN [integration_object]
					ON [integration].integration_id = [integration_object].integration_id
				INNER JOIN [integration_fields]
					ON [integration_object].integration_id = [integration_fields].integration_id	
						AND [integration_object].integration_object_id = [integration_fields].integration_object_id				
		WHERE UPPER([integration].integration_name) = UPPER(@integration_name)
				AND UPPER([integration_object].integration_object_name) = UPPER(@integration_object_name)
				AND [integration_fields].integration_field_default_value IS NOT NULL

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END