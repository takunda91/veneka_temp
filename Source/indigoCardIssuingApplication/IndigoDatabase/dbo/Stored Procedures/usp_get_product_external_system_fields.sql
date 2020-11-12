-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_external_system_fields] 
	-- Add the parameters for the stored procedure here
	@external_system_type_id int, 
	@product_id int,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT PES.external_system_field_id,PES.product_id,PES.field_value, [external_system_fields].external_system_id,[external_system_fields].field_name,[external_systems].external_system_type_id
	FROM [product_external_system] PES INNER JOIN [external_system_fields]
			ON PES.external_system_field_id = [external_system_fields].external_system_field_id
		INNER JOIN [external_systems]
			ON [external_system_fields].external_system_id = [external_systems].external_system_id
	WHERE [external_systems].external_system_type_id =  COALESCE(@external_system_type_id,[external_systems].external_system_type_id)
			AND PES.product_id = @product_id
END