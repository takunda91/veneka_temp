CREATE PROCEDURE [dbo].[usp_get_feerevenue_report]
	-- Add the parameters for the stored procedure here
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@product_id int=null,
	@fromdate datetimeoffset,
	@todate datetimeoffset,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null
		Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
		SELECT  inter.branch_id,
    inter.branch_code, 
    sum(inter.zero_no_fee) as 'zero_no_fee',
    sum(inter.amended_fee) as 'amended_fee',
    sum(inter.full_fee) as 'full_fee', 
    sum(inter.fee_charged)as 'fee_charged'
    ,inter.currency_id,inter.[currency_code],
    inter.product,
	inter.excuted_datetime
    from (
  SELECT [branch].branch_id,
    [branch].branch_code+'-'+branch.branch_name as 'branch_code', 
    case when fee.fee_waiver_YN =1 then count([branch_card_status_current].card_id) else 0 end as 'zero_no_fee',
    0  as 'amended_fee',
    0  as 'full_fee', 
   case when  sum(fee.fee_charged) is null then 0 else sum(fee.fee_charged) end as 'fee_charged'
    ,[currency].currency_id,[currency_code],
    issuer_product.product_code+'-'+issuer_product.product_name as product
    ,issuer_product.product_id,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
   FROM [branch_card_status_current] 
     INNER JOIN [cards]
      ON [branch_card_status_current].card_id = [cards].card_id
	  INNER JOIN [fee_charged] as fee
						ON fee.fee_id = [cards].fee_id
     INNER JOIN [branch]
      ON [cards].branch_id = [branch].branch_id
     INNER JOIN [product_currency] 
     on [product_currency].product_id=[cards].product_id
     INNER JOIN [currency] 
     ON [currency].currency_id = [product_currency].currency_id
     INNER JOIN [issuer_product] 
     ON [cards].product_id=[issuer_product].product_id
   WHERE [branch_card_status_current].branch_card_statuses_id = 6
     and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id) 
     AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)     
     AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
     AND CAST(SWITCHOFFSET( [branch_card_status_current].[status_date],@UserTimezone) as datetime)  >= @fromdate 
     AND CAST(SWITCHOFFSET( [branch_card_status_current].[status_date],@UserTimezone) as datetime) <= @todate
     --AND [cards].fee_waiver_YN = 0
      group by [branch].branch_id,[branch].branch_code,branch_name,
       fee.fee_overridden_YN,fee.fee_waiver_YN,[currency].currency_id,
       [currency_code],issuer_product.product_code,issuer_product.product_name,issuer_product.product_id

   union 
   SELECT [branch].branch_id,
    [branch].branch_code+'-'+branch.branch_name as 'branch_code', 
    0 as 'zero_no_fee',
    case when fee.fee_overridden_YN =1 then count([branch_card_status_current].card_id) else 0 end as 'amended_fee',
    0 as 'full_fee', 
    0 as 'fee_charged'
    ,[currency].currency_id,[currency_code],
    issuer_product.product_code+'-'+issuer_product.product_name as product
    ,issuer_product.product_id,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
   FROM [branch_card_status_current] 
     INNER JOIN [cards]
      ON [branch_card_status_current].card_id = [cards].card_id
	  INNER JOIN [fee_charged] as fee
						ON fee.fee_id = [cards].fee_id
     INNER JOIN [branch]
      ON [cards].branch_id = [branch].branch_id
     INNER JOIN [product_currency] 
     on [product_currency].product_id=[cards].product_id
     INNER JOIN [currency] 
     ON [currency].currency_id = [product_currency].currency_id
     INNER JOIN [issuer_product] 
     ON [cards].product_id=[issuer_product].product_id
   WHERE [branch_card_status_current].branch_card_statuses_id = 6
     and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id) 
     AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)     
     AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
     AND CAST(SWITCHOFFSET( [branch_card_status_current].[status_date],@UserTimezone) as datetime)  >= @fromdate 
     AND CAST(SWITCHOFFSET( [branch_card_status_current].[status_date],@UserTimezone) as datetime) <= @todate
     --AND [cards].fee_waiver_YN = 0
      group by [branch].branch_id,[branch].branch_code,branch_name,
       fee.fee_overridden_YN,fee.fee_waiver_YN,[currency].currency_id,
       [currency_code],issuer_product.product_code,issuer_product.product_name,issuer_product.product_id
  union 
  SELECT [branch].branch_id,
    [branch].branch_code+'-'+branch.branch_name as 'branch_code', 
   0 as 'zero_no_fee',
    0 as 'amended_fee',
    case when fee.fee_overridden_YN =0 then count([branch_card_status_current].card_id) else 0 end as 'full_fee', 
    0 as 'fee_charged'
    ,[currency].currency_id,[currency_code],
    issuer_product.product_code+'-'+issuer_product.product_name as product
    ,issuer_product.product_id,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
   FROM [branch_card_status_current] 
     INNER JOIN [cards]
      ON [branch_card_status_current].card_id = [cards].card_id
	  INNER JOIN [fee_charged] as fee
						ON fee.fee_id = [cards].fee_id
     INNER JOIN [branch]
      ON [cards].branch_id = [branch].branch_id
     INNER JOIN [product_currency] 
     on [product_currency].product_id=[cards].product_id
     INNER JOIN [currency] 
     ON [currency].currency_id = [product_currency].currency_id
     INNER JOIN [issuer_product] 
     ON [cards].product_id=[issuer_product].product_id
   WHERE [branch_card_status_current].branch_card_statuses_id = 6
     and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id) 
     AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)     
     AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
     AND CAST(SWITCHOFFSET( [branch_card_status_current].[status_date],@UserTimezone) as datetime)  >= @fromdate 
     AND CAST(SWITCHOFFSET( [branch_card_status_current].[status_date],@UserTimezone) as datetime) <= @todate
     --AND [cards].fee_waiver_YN = 0
      group by [branch].branch_id,[branch].branch_code,branch_name,
       fee.fee_overridden_YN,fee.fee_waiver_YN,[currency].currency_id,
       [currency_code],issuer_product.product_code,issuer_product.product_name,issuer_product.product_id
       ) inter

       group by  inter.branch_id,
    inter.branch_code, 
    
    inter.currency_id,inter.[currency_code],
    inter.product,
	inter.excuted_datetime
	
	END
