Create PROCEDURE [dbo].[usp_get_customer_issuedcards]
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

SELECT		ip.product_name 		, CASE 			WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY(c.card_number)),6,4) 			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY(c.card_number))		  END AS 'card_number'		, c.card_request_reference AS card_reference_number		, [customer_account_cards].card_id		, CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_first_name)) as 'first_name'		, CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_last_name)) as 'last_name'		, CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_account_number)) AS account_number	FROM  [customer_account] AS ca
	INNER JOIN [customer_account_cards] 
						ON [customer_account_cards].customer_account_id = ca.customer_account_id 
						 					INNER JOIN cards as c	ON [customer_account_cards].card_id = c.card_id							INNER JOIN [branch_card_status_current] ON [branch_card_status_current].card_id = [customer_account_cards].card_id			INNER JOIN issuer_product as ip ON c.product_id = ip.product_id		INNER JOIN [branch] ON [branch].branch_id = c.branch_id		INNER JOIN [issuer] ON [issuer].[issuer_id] = [branch].issuer_id
		Where
ip.issuer_id =COALESCE(@issuer_id, ip.issuer_id)

AND (@customeraccountno IS NULL OR CONVERT(VARCHAR(max),DECRYPTBYKEY(ca.customer_account_number)) LIKE @customeraccountno) 

AND [branch_card_status_current].branch_card_statuses_id = 6 
--AND c.product_id =COALESCE(@product_id, c.product_id)



-- Insert statements for procedure here

CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END