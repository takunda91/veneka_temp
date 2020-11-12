-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_printfields_byproductid]
	@Productid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
		
		SELECT  distinct      product_fields.field_name, print_field_types.print_field_name, product_fields.product_field_id,product_fields.mapped_name,product_fields.[editable],
		CONVERT(VARCHAR(max),DECRYPTBYKEY('')) as 'Value',product_fields.deleted
					FROM  product_fields 
					--left JOIN
     --                    customer_fields ON product_fields.product_field_id = customer_fields.product_field_id 
						 INNER JOIN
                         print_field_types ON product_fields.print_field_type_id = print_field_types.print_field_type_id
						 where product_fields.product_id=@Productid 
						 --and Upper(product_fields.Mapped_Name)<>'IND_SYS_NOC'
 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END