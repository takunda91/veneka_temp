CREATE PROCEDURE [dbo].[usp_get_issuecardsummaryreport_issuedlifecyle]
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

		SET @todate = DATEADD(d, 1, @todate)
		
Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	SELECT INTERX.issuer_id,
		   branch_code,
		   INTERX.branch_id,
		   INTERX.card_issue_reason_id,
		   INTERX.card_issuer_reason_name,
		   COUNT(INTER.card_id)	AS CardCount,issuer.issuer_name as 'issuer_name',
		   product_code+'-'+product_name as product
		   ,product_code
		   ,product_id
	FROM 
		-- this sub select fetches all cards belonging to a branch and currently in issued status
		(SELECT [branch].branch_id, branch_card_status.card_id, [customer_account].card_issue_reason_id,
		[Issuer_product].product_code,[Issuer_product].product_name 
		   ,[Issuer_product].product_id
			FROM branch_card_status 		

					INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = branch_card_status.card_id 
						INNER JOIN [customer_account]
						ON [customer_account_cards].customer_account_id = [customer_account].customer_account_id 
					INNER JOIN [cards]
						ON branch_card_status.card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id						
					INNER JOIN [Issuer_product] 
						ON [cards].product_id=[issuer_product].product_id
			WHERE branch_card_status.branch_card_statuses_id = 6
				  AND CAST(SWITCHOFFSET(branch_card_status.[status_date],@UserTimezone) AS datetime)  >= @fromdate 
				  AND CAST(SWITCHOFFSET(branch_card_status.[status_date],@UserTimezone) AS datetime) <= @todate
				   union
				  SELECT [branch].branch_id, [branch_card_status_audit].card_id, [customer_account].card_issue_reason_id,
		[Issuer_product].product_code,[Issuer_product].product_name 
		   ,[Issuer_product].product_id
			FROM [branch_card_status_audit] 
					INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = [branch_card_status_audit].card_id 
					INNER JOIN [customer_account] ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id
					
					INNER JOIN [cards]
						ON [branch_card_status_audit].card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id						
					INNER JOIN [Issuer_product] 
						ON [cards].product_id=[issuer_product].product_id
			WHERE [branch_card_status_audit].branch_card_statuses_id = 6
				  AND CAST(SWITCHOFFSET([branch_card_status_audit].[status_date],@UserTimezone) AS datetime)  >= @fromdate 
				  AND CAST(SWITCHOFFSET([branch_card_status_audit].[status_date],@UserTimezone) AS datetime) <= @todate
		) AS INTER						
		RIGHT OUTER JOIN 		
		--This Sub Select creates a cartesian product of branch and card issue reason	
		(SELECT issuer_id, [branch].branch_id, branch.branch_name+'-'+ branch_code as branch_code, [card_issue_reason].[card_issue_reason_id],
				 language_text as 'card_issuer_reason_name'
			FROM [branch],[card_issue_reason]
					INNER JOIN [card_issue_reason_language]  
						ON [card_issue_reason].[card_issue_reason_id] = [card_issue_reason_language].[card_issue_reason_id]
						   AND [card_issue_reason_language].language_id = 0
						   Where    [branch].branch_status_id = 0
		 	)  INTERX
		ON INTER.branch_id = INTERX.branch_id
			AND INTER.[card_issue_reason_id] = INTERX.[card_issue_reason_id] 
			inner join issuer on issuer.issuer_id=INTERX.issuer_id
	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		AND INTERX.branch_id = COALESCE(@branch_id,  INTERX.branch_id)	
		AND INTERX.branch_code <> '' 		
		AND INTER.product_id=COALESCE(@product_id, INTER.product_id)
	GROUP BY INTERX.issuer_id,  INTERX.branch_id,INTERX.branch_code,  INTERX.[card_issue_reason_id],INTERX.[card_issuer_reason_name],issuer_name,product_code,product_name ,product_id
	ORDER BY issuer_name, INTERX.branch_code, INTERX.[card_issue_reason_id]
END