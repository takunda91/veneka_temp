-- =============================================
-- Author:		sandhya
-- Create date: <Create Date,,>
-- Description:Inserting external fields to product
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_product_external_systems] 
	-- Add the parameters for the stored procedure here
	@product_id int,
	@external_system_fields as dbo.[product_external_fields_array] READONLY,	
	@audit_user_id BIGINT,
	@audit_workstation varchar(100)
AS
BEGIN
	
	SET NOCOUNT ON;

	

			INSERT INTO product_external_system
						(product_id, external_system_field_id, field_value)
			SELECT @product_id, esx.external_system_field_id, field_value
			FROM @external_system_fields esx

			

END