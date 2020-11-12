CREATE PROCEDURE [dbo].[usp_card_expiry_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@product_id int=null,
	@date_from datetimeoffset,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	--SET @date_from = DATEADD(M, 1, @date_from)
	--set @date_from = convert(datetime, @date_from, 108)

	DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT 
			DISTINCT  
			CASE 
				WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(100),DECRYPTBYKEY([cards].card_number)),6,4) 
				ELSE CONVERT(VARCHAR(100),DECRYPTBYKEY([cards].card_number))
			END AS 'card_number'
			, cards.card_request_reference AS card_reference_number,
			CAST(SWITCHOFFSET(CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)),@UserTimezone) as datetime)	AS card_expiry_date
			, [issuer].issuer_name
			, [issuer].issuer_code
			, [branch].branch_name
			, branch.branch_code
			,issuer_product.product_code+'-'+issuer_product.product_name as product
			,issuer_product.product_id
		FROM [cards]								
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id	
					INNER JOIN [issuer_product] 
		ON [cards].product_id=[issuer_product].product_id
		WHERE [cards].card_issue_method_id = 0
				AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
				AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
				AND DATEPART(m, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)))) = DATEPART(m, @date_from)
				AND DATEPART(yy, CONVERT(DATETIME, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)))) = DATEPART(yy, @date_from)
				AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
		ORDER BY issuer_name, issuer_code, branch_name, branch_code, card_expiry_date

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END