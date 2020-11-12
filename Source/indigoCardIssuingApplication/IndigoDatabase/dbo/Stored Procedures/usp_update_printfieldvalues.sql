-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_printfieldvalues]	
	@product_fields as dbo.key_binary_value_array READONLY,
	@customeraccountid int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	--if( (select count(*) from customer_fields where customer_fields.product_field_id=@product_field_id 
	--and customer_fields.customer_account_id=@customeraccountid)>0)
	--	BEGIN
	--		 update customer_fields set value=	ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@value))
	--		  where [product_field_id]= @product_field_id
			 
	
	--	 END
	-- ELSE
		 BEGIN

				Delete from customer_fields where customer_account_id = @customeraccountid
				--Update Product Fields
					INSERT INTO customer_fields (customer_account_id, product_field_id, value)
						SELECT @customeraccountid, pf.[key], ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CAST(pf.[value] as varbinary(max)))
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 0 
						--and product_fields.deleted=0

					--		-- if values in printfields updating 
					--if((select count(*) from customer_fields as c
					--	inner join product_fields as  p on c.product_field_id=p.product_field_id
					--where 	 c.customer_account_id= @customeraccountid and Upper(p.Mapped_Name)='IND_SYS_NOC' )>0)
					--BEGIN
		
					--update c 			
					--set c.value=	ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@name_on_card))
					--from  customer_fields as c inner join  product_fields as  p on c.product_field_id=p.product_field_id
					--  where c.customer_account_id=@customeraccountid 
					--  and Upper(p.Mapped_Name)='IND_SYS_NOC'

					--END
					Delete from customer_image_fields where customer_account_id = @customeraccountid

					--Update Product Image Fields
					INSERT INTO customer_image_fields (customer_account_id, product_field_id, value)
						SELECT @customeraccountid, pf.[key], pf.[value]
						FROM @product_fields pf INNER JOIN product_fields
							ON pf.[key] = product_fields.product_field_id
						WHERE product_fields.print_field_type_id = 1 
						--and product_fields.deleted=0
		 END

	  CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END