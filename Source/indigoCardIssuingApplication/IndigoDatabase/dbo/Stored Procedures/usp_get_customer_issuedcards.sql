﻿Create PROCEDURE [dbo].[usp_get_customer_issuedcards]
@issuer_id int =null,
@customeraccountno nvarchar(50),
@product_id int =null,
@audit_user_id bigint,
@audit_workstation varchar(100)
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;
IF @customeraccountno = '' OR @customeraccountno IS NULL
SET @customeraccountno = NULL
ELSE
SET @customeraccountno = '%' + @customeraccountno + '%'


OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate;

DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

SELECT
	INNER JOIN [customer_account_cards] 
						ON [customer_account_cards].customer_account_id = ca.customer_account_id 
						 
		Where
ip.issuer_id =COALESCE(@issuer_id, ip.issuer_id)

AND (@customeraccountno IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_account_number)) LIKE @customeraccountno) 

AND [branch_card_status_current].branch_card_statuses_id = 6 
--AND c.product_id =COALESCE(@product_id, c.product_id)



-- Insert statements for procedure here

CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END