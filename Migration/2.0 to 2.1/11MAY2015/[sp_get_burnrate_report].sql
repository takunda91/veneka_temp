USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_burnrate_report]    Script Date: 2015/05/11 04:09:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [sp_get_burnrate_report] null,3,0,null
ALTER PROCEDURE [dbo].[sp_get_burnrate_report]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@REPORT_DATE datetime
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
--SELECT @REPORT_DATE = '2004-09-21T00:00:00'
SELECT @WEEK_BEGINING = 'MONDAY'

set @REPORT_DATE = convert(datetime, getdate(), 126)

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


DECLARE @WEEK_START_DATE DATETIME, @WEEK_END_DATE DATETIME
--GET THE WEEK START DATE
set  @WEEK_START_DATE = @REPORT_DATE - (DATEPART(DW,  @REPORT_DATE) - 1) 

--GET THE WEEK END DATE
set  @WEEK_END_DATE = @REPORT_DATE + (7 - DATEPART(DW,  @REPORT_DATE))

select interx.branch_id, interx.branch_code , sum(interx.oneeighty_d) as '180 d' ,sum(interx.oneeighty_d) as '90 d',sum(interx.Week_one) as 'Week 1', sum(interx.Week_two) as 'Week 2',sum(interx.Week_three) as 'Week 3',sum(interx.Week_four) as 'Week 4',sum(interx.Current_Card_Stock) as 'Current Card Stock',sum(interx.Weeks_remaining) as 'Weeks remaining'

from (
SELECT distinct [branch].branch_id, branch.branch_code ,count(branch_card_status.card_id) as 'oneeighty_d',0 as 'ninty_d',0 as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
		FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
		INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 and 
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] <=  @REPORT_DATE
				  AND branch_card_status.[status_date] >= DATEADD(d, -180,@REPORT_DATE)
				  group by [branch].branch_id, branch.branch_code 

union

SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',count(branch_card_status.card_id) as 'oneeighty_d',0 as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
			INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
			INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
			INNER JOIN [branch] 	ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 and
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] <=  @REPORT_DATE
				  AND branch_card_status.[status_date] >= DATEADD(d, -90,@REPORT_DATE)
				  group by [branch].branch_id, branch.branch_code 

union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'oneeighty_d', count(branch_card_status.card_id) as 'Week_one', 0 as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
			INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
			INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
			INNER JOIN [branch]		ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 and 
			[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >=  @WEEK_START_DATE
				  AND branch_card_status.[status_date] <=  @WEEK_END_DATE
				  group by [branch].branch_id, branch.branch_code 

union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'oneeighty_d', 0  as 'Week_one', count(branch_card_status.card_id)   as 'Week_two',0  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
        INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6  
			and [branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)		
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >= DATEADD(d, -7,@WEEK_START_DATE)
				  AND branch_card_status.[status_date] <= DATEADD(d, -1,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code 

union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'oneeighty_d', 0  as 'Week_one',0 as  'Week_two',count(branch_card_status.card_id)  as 'Week_three',0  as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account]		ON [customer_account].card_id = branch_card_status.card_id
		INNER JOIN [branch]	ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6   
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >= DATEADD(d, -14,@WEEK_START_DATE)
				  AND branch_card_status.[status_date] <= DATEADD(d, -8,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code 
union 
SELECT  distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'oneeighty_d', 0  as 'Week_one',0 as  'Week_two', 0 as 'Week_three',count(branch_card_status.card_id) as 'Week_four',0 as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [cards]
		INNER JOIN 	  dbo.branch_card_status  	ON  dbo.branch_card_status .card_id = [cards].card_id
		INNER JOIN [customer_account] 	ON [customer_account].card_id = branch_card_status.card_id
		INNER JOIN [branch] ON [cards].branch_id = [branch].branch_id
			WHERE branch_card_status.branch_card_statuses_id = 6 	
			and[branch].branch_id = COALESCE(@branch_id,  [branch].branch_id)	
			AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  AND branch_card_status.[status_date] >= DATEADD(d, -21,@WEEK_START_DATE)
				  AND branch_card_status.[status_date] <= DATEADD(d, -15,@WEEK_START_DATE)
				  group by [branch].branch_id, branch.branch_code 
union 
SELECT distinct [branch].branch_id, branch.branch_code , 0 as 'oneeighty_d',0 as 'oneeighty_d', 0  as 'Week_one',0 as  'Week_two', 0 as 'Week_three',0 as 'Week_four',count([branch_card_status_current].card_id)  as 'Current_Card_Stock',0 as 'Weeks_remaining'
			FROM [branch_card_status_current] 
					INNER JOIN [cards]
						ON [branch_card_status_current].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
			WHERE ([branch_card_status_current].branch_card_statuses_id = 1	or [branch_card_status_current].branch_card_statuses_id = 0)
			and[branch].branch_id = COALESCE(null,  [branch].branch_id)		
			 AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
				  group by [branch].branch_id, branch.branch_code 
				 
) AS interx
group by interx.branch_id, interx.branch_code
END
