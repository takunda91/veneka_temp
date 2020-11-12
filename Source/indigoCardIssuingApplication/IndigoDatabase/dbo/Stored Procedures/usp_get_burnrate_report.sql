CREATE PROCEDURE [dbo].[usp_get_burnrate_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@product_id int=null,
	--@REPORT_DATE datetime=null,
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

		DECLARE  @WEEK_BEGINING VARCHAR(10)
		Declare @REPORT_DATE datetime
--SELECT @REPORT_DATE = '2004-09-21T00:00:00'
set @REPORT_DATE=GETDATE()
SELECT @WEEK_BEGINING = 'MONDAY'
if(@REPORT_DATE is null or @REPORT_DATE='')
set @REPORT_DATE = convert(datetime, getdate(), 126)
else
set @REPORT_DATE = convert(datetime, @REPORT_DATE, 126)

IF @WEEK_BEGINING = 'MONDAY' 
    SET DATEFIRST 1 
ELSE IF @WEEK_BEGINING = 'TUESDAY' 
    SET  DATEFIRST 2 
ELSE IF @WEEK_BEGINING = 'WEDNESDAY'
    SET  DATEFIRST 3 
ELSE IF @WEEK_BEGINING =  'THURSDAY'
    SET  DATEFIRST 4 
ELSE IF @WEEK_BEGINING =  'FRIDAY'
    SET  DATEFIRST 5 
ELSE IF @WEEK_BEGINING =  'SATURDAY'
    SET  DATEFIRST 6 
ELSE IF @WEEK_BEGINING =  'SUNDAY'
    SET  DATEFIRST 7 

DECLARE @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

DECLARE @WEEK_START_DATE DATETIME, @WEEK_END_DATE DATETIME
--GET THE WEEK START DATE
set  @WEEK_START_DATE = @REPORT_DATE - (DATEPART(DW,  @REPORT_DATE) - 1) 

--GET THE WEEK END DATE
set  @WEEK_END_DATE = @REPORT_DATE + (7 - DATEPART(DW,  @REPORT_DATE))

select interx.product,interx.product_id, interx.branch_id, interx.branch_code , sum(interx.oneeighty_d) as '180 d' ,sum(interx.oneeighty_d) as '90 d',sum(interx.Week_one) as 'Week 1', sum(interx.Week_two) as 'Week 2',sum(interx.Week_three) as 'Week 3',sum(interx.Week_four) as 'Week 4',sum(interx.Current_Card_Stock) as 'Current Card Stock',sum(interx.Weeks_remaining) as 'Weeks remaining',CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'

from (
SELECT distinct issuer_product.product_code+'-'+issuer_product.product_name as product,issuer_product.product_id, [branch].branch_id,branch.branch_name+'-'+ branch.branch_code as 'branch_code' ,count(branch_card_status.card_id) as 'oneeighty_d',0 as 'ninty_d',0 as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
		FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account_cards] 	ON [customer_account_cards].card_id = branch_card_status.card_id
		INNER JOIN [customer_account] 	ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id
		INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer_product] on cards.product_id=issuer_product.product_id
			WHERE branch_card_status.branch_card_statuses_id = 6 and 
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
			 AND cards.product_id = COALESCE(@product_id, cards.product_id)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) <=  @REPORT_DATE
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) >= DATEADD(d, -180,@REPORT_DATE)
				  group by [branch].branch_id, branch.branch_code ,branch.branch_name,product_name,issuer_product.product_id,issuer_product.product_code

union

SELECT distinct issuer_product.product_name,issuer_product.product_id,[branch].branch_id, branch.branch_name+'-'+ branch.branch_code as 'branch_code', 0 as 'oneeighty_d',count(branch_card_status.card_id) as 'ninty_d',0 as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
			INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
			INNER JOIN [customer_account_cards] 	ON [customer_account_cards].card_id = branch_card_status.card_id
			INNER JOIN [customer_account] 	ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id
			INNER JOIN [branch] 	ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer_product] on cards.product_id=issuer_product.product_id

			WHERE branch_card_status.branch_card_statuses_id = 6 and

			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
			  AND cards.product_id = COALESCE(@product_id, cards.product_id)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) <=  @REPORT_DATE
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime)>= DATEADD(d, -90,@REPORT_DATE)
				  group by [branch].branch_id, branch.branch_code ,branch.branch_name,product_name,issuer_product.product_id

union 
SELECT distinct  issuer_product.product_name,issuer_product.product_id, [branch].branch_id,branch.branch_name+'-'+ branch.branch_code as 'branch_code' , 0 as 'oneeighty_d',0 as 'ninty_d', count(branch_card_status.card_id) as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
			INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
			INNER JOIN [customer_account_cards] 	ON [customer_account_cards].card_id = branch_card_status.card_id
			INNER JOIN [customer_account] 	ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id
			INNER JOIN [branch]		ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer_product] on cards.product_id=issuer_product.product_id

			WHERE branch_card_status.branch_card_statuses_id = 6 and 
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
			  AND cards.product_id = COALESCE(@product_id, cards.product_id)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) >=  @WEEK_START_DATE
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) <=  @WEEK_END_DATE
				  group by [branch].branch_id, branch.branch_code ,branch.branch_name,product_name,issuer_product.product_id

union 
SELECT distinct issuer_product.product_name,issuer_product.product_id,  [branch].branch_id, branch.branch_name+'-'+ branch.branch_code as 'branch_code' , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one', count(branch_card_status.card_id)   as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account_cards] 	ON [customer_account_cards].card_id = branch_card_status.card_id
		INNER JOIN [customer_account] 	ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id
        INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer_product] on cards.product_id=issuer_product.product_id

			WHERE branch_card_status.branch_card_statuses_id = 6  
			and [branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
			 AND cards.product_id = COALESCE(@product_id, cards.product_id)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) >= DATEADD(d, -7,@WEEK_START_DATE)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) <= DATEADD(d, -1,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code ,branch.branch_name,product_name,issuer_product.product_id

union 
SELECT distinct issuer_product.product_name,issuer_product.product_id, [branch].branch_id, branch.branch_name+'-'+ branch.branch_code as 'branch_code' , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one',0 as  'Week_two',count(branch_card_status.card_id)  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account_cards] 	ON [customer_account_cards].card_id = branch_card_status.card_id
		INNER JOIN [customer_account] 	ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id
		INNER JOIN [branch]	ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer_product] on cards.product_id=issuer_product.product_id

			WHERE branch_card_status.branch_card_statuses_id = 6   
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
			 AND cards.product_id = COALESCE(@product_id, cards.product_id)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) >= DATEADD(d, -14,@WEEK_START_DATE)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) <= DATEADD(d, -8,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code ,branch.branch_name,product_name,issuer_product.product_id
union 
SELECT  distinct issuer_product.product_name,issuer_product.product_id,[branch].branch_id, branch.branch_name+'-'+ branch.branch_code as 'branch_code' , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one',0 as  'Week_two', 0 as 'Week_three',count(branch_card_status.card_id) as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account_cards] 	ON [customer_account_cards].card_id = branch_card_status.card_id
		INNER JOIN [customer_account] 	ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id

		INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer_product] on cards.product_id=issuer_product.product_id

			WHERE branch_card_status.branch_card_statuses_id = 6 	
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
			 AND cards.product_id = COALESCE(@product_id, cards.product_id)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) >= DATEADD(d, -21,@WEEK_START_DATE)
				  AND cast(SWITCHOFFSET( branch_card_status.[status_date],@UserTimezone) as datetime) <= DATEADD(d, -15,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code,branch.branch_name ,product_name,issuer_product.product_id
union 
SELECT distinct issuer_product.product_name,issuer_product.product_id,[branch].branch_id, branch.branch_name+'-'+ branch.branch_code as 'branch_code' , 0 as 'oneeighty_d',0 as 'ninty_d', 0  as 'Week_one',0 as  'Week_two', 0 as 'Week_three',0 as 'Week_four',count([branch_card_status_current].card_id)  as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [branch_card_status_current] 
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer_product] on cards.product_id=issuer_product.product_id

			WHERE ([branch_card_status_current].branch_card_statuses_id = 1	or [branch_card_status_current].branch_card_statuses_id = 0)
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
			 AND cards.product_id = COALESCE(@product_id, cards.product_id)

				  group by [branch].branch_id, branch.branch_code ,branch.branch_name,product_name,issuer_product.product_id
				 
) AS interx
where interx.branch_id = COALESCE(@branch_id,  interx.branch_id)		
			 --AND interx.issuer_id = COALESCE(@issuer_id, interx.issuer_id)
			 AND interx.product_id = COALESCE(@product_id, interx.product_id)

group by interx.branch_id, interx.branch_code,interx.product,interx.product_id
END



GO


