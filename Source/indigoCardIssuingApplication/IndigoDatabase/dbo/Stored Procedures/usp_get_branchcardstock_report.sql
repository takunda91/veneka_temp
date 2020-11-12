-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [usp_get_branchcardstock_report] null,18,0,2,'veneka'

CREATE PROCEDURE [dbo].[usp_get_branchcardstock_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@product_id int=null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
	WITH RECOMPILE
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
		  ,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		, [cards].card_request_reference as 'card_request_reference'
		--, Convert(datetime, CONVERT(varchar(max),DECRYPTBYKEY([cards].card_expiry_date))) as 'card_expiry_date'
		,
Convert(datetime,SWITCHOFFSET(CONVERT(varchar(max),DECRYPTBYKEY([cards].card_expiry_date)),@UserTimezone)) as 'card_expiry_date'
		, cast(SWITCHOFFSET( [dist_batch].date_created,@UserTimezone) as datetime) as'card_production_date'
		,issuer_product.product_code+'-'+issuer_product.product_name as product
		,issuer_product.product_id,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
	FROM 
		branch_card_status_current
		INNER JOIN [cards] 
			ON branch_card_status_current.card_id = [cards].card_id
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
		INNER JOIN [dist_batch]
			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
					--AND [dist_batch].dist_batch_type_id = 0
		INNER JOIN [branch] 
			ON [cards].branch_id = [branch].branch_id
				AND [branch].branch_type_id <> 0 
		INNER JOIN [issuer] 
			ON issuer.issuer_id = branch.issuer_id
				INNER JOIN [issuer_product] 
		ON [cards].product_id=[issuer_product].product_id
	WHERE
		(branch_card_status_current.branch_card_statuses_id = 1 
			OR branch_card_status_current.branch_card_statuses_id = 0)
		AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		AND [branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
		AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
	ORDER BY
		[branch].branch_id
		
			
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
