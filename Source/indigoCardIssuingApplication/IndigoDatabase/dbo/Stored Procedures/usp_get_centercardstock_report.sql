-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_centercardstock_report] null,3,0,2,'veneka'

CREATE  PROCEDURE [dbo].[usp_get_centercardstock_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@product_id int =null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
    -- Insert statements for procedure here
	 OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
	
	SELECT distinct 
		[branch].branch_id,branch.branch_code
		,CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, [cards].card_request_reference as 'card_request_reference'
		, CONVERT(DATETIME,SWITCHOFFSET(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)),@UserTimezone) ) as 'card_expiry_date'
		, CONVERT(DATETIME, SWITCHOFFSET(CONVERT(VARCHAR(max),([cards].card_activation_date)),@UserTimezone) )  as 'card_production_date'
		,issuer_product.product_code+'-'+issuer_product.product_name as product
		,issuer_product.product_id,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
	 
		FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [cards].card_id = [branch_card_status_current].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer] 
			ON issuer.issuer_id = branch.issuer_id
			INNER JOIN [issuer_product] 
		ON [cards].product_id=[issuer_product].product_id
	WHERE
		 [branch_card_status_current].branch_card_statuses_id = 0		
		AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		AND [branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
		AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
	ORDER BY
		[branch].branch_id
		
			
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


