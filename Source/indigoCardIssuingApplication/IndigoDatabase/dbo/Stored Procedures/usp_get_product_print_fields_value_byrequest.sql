CREATE PROCEDURE [dbo].[usp_get_product_print_fields_value_byrequest]
	@request_id bigint
AS
BEGIN
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

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
		, CAST(DECRYPTBYKEY(customer_fields.value) as image) value,
		print_field_types.print_field_name
		, product_fields.printable
		, product_fields.printside
	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
		inner JOIN customer_fields ON customer_fields.product_field_id = product_fields.product_field_id
		inner JOIN customer_account_requests ON customer_account_requests.customer_account_id = customer_fields.customer_account_id
		--LEFT JOIN cards ON cards.card_id = customer_account.card_id
	WHERE customer_account_requests.request_id = @request_id
	UNION ALL
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
		, customer_image_fields.value,
		print_field_types.print_field_name
		, product_fields.printable
		, product_fields.printside
	FROM
		product_fields
		 INNER JOIN print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id

		inner JOIN customer_image_fields ON customer_image_fields.product_field_id = product_fields.product_field_id
		inner JOIN customer_account_requests ON customer_account_requests.customer_account_id = customer_image_fields.customer_account_id
		--LEFT JOIN cards ON cards.card_id = customer_account.card_id
	WHERE customer_account_requests.request_id = @request_id
	
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END

