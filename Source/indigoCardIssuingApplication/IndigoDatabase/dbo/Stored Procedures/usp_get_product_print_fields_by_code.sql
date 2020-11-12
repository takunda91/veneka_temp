-- =============================================
-- Author:		LTladi 
-- Create date: 20151116
-- Description:	Get product print fields by product code
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_print_fields_by_code]
	@product_code varchar(50),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		product_fields.product_field_id
		, product_fields.product_id
		, product_fields.field_name
		, product_fields.print_field_type_id
		, product_fields.X
		, product_fields.Y
		, product_fields.width
		, product_fields.height
		, product_fields.font
		, product_fields.font_size
		, product_fields.mapped_name
		, product_fields.editable
		, product_fields.deleted
		, product_fields.label
		, product_fields.max_length
		, cast(null as varbinary) as value,
		print_field_types.print_field_name
		, product_fields.printable
		, product_fields.printside
	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
	
		 INNER JOIN issuer_product 	ON product_fields.product_id = issuer_product.product_id
	WHERE
		issuer_product.product_code = @product_code
		AND product_fields.deleted = 0

END