CREATE PROCEDURE [dbo].[usp_get_Spoilcardsummaryreport_issuedlifecycle]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@fromdate datetimeoffset,
	@todate datetimeoffset,	
	@product_id int=null,
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

	DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	SET @todate = DATEADD(DD, 1, @todate)

	DECLARE @cards_spoilt TABLE
	(
		card_id bigint PRIMARY KEY,
		status_date datetimeoffset(7) not null,
		branch_id int not null,
		[user_id] bigint not null,
		branch_card_statuses_id int not null,
		branch_card_code_id int not null
	)

	INSERT INTO @cards_spoilt (card_id, status_date, branch_id, [user_id], branch_card_statuses_id, branch_card_code_id)
	SELECT [branch_card_status].card_id, status_date, branch_id, [user_id],branch_card_statuses_id, branch_card_code_id
	FROM [branch_card_status]
	WHERE [branch_card_status].branch_card_statuses_id = 7
		AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone)as datetime) >= @fromdate 
		AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone) AS datetime) <= @todate
	EXCEPT
	SELECT [branch_card_status].card_id, [branch_card_status].status_date, [branch_card_status].branch_id, [branch_card_status].[user_id],
			[branch_card_status].branch_card_statuses_id, [branch_card_status].branch_card_code_id
	FROM [branch_card_status_audit] INNER JOIN [branch_card_status]
			ON [branch_card_status].card_id = [branch_card_status_audit].card_id
	WHERE [branch_card_status_audit].branch_card_statuses_id = 6 
		  AND [branch_card_status].branch_card_statuses_id = 7
		AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone)as datetime) >= @fromdate 
		AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone) AS datetime) <= @todate


	SELECT INTERX.issuer_id,
		   INTERX.branch_code,
		   INTERX.branch_id,
		   INTERX.branch_card_code_id,
		   INTERX.Spoil_reason_name,
		   COUNT(INTER.card_id)	AS CardCount,
		   issuer.issuer_name as 'issuer_name',
		     product_code+'-'+product_name as product
		   ,product_id
	FROM 
		-- this sub select fetches all cards belonging to a branch and currently in issued status
		(SELECT [branch].branch_id, spoilt_cards.card_id, spoilt_cards.branch_card_code_id,	[Issuer_product].product_code,[Issuer_product].product_name 
		   ,[Issuer_product].product_id
			FROM @cards_spoilt spoilt_cards 
					INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = spoilt_cards.card_id 
					INNER JOIN [cards]
						ON spoilt_cards.card_id = [cards].card_id
					INNER JOIN [branch]
						ON [cards].branch_id = [branch].branch_id
					INNER JOIN [Issuer_product] 
						ON [cards].product_id=[issuer_product].product_id						  
		) AS INTER						
		RIGHT OUTER JOIN 		
		--This Sub Select creates a cartesian product of branch and card issue reason	
		(SELECT issuer_id, [branch].branch_id, branch.branch_name+'-'+ branch_code as branch_code, branch_card_codes.branch_card_code_id,
				 language_text as 'Spoil_reason_name'
			FROM [branch], branch_card_codes
					INNER JOIN branch_card_codes_language 
						ON branch_card_codes.branch_card_code_id = branch_card_codes_language.branch_card_code_id
						   AND branch_card_codes_language.language_id = @language_id 						  
						   AND  branch_card_codes.is_exception = 1
			Where    [branch].branch_status_id = 0
		 	)  INTERX
		ON INTER.branch_id = INTERX.branch_id
			AND INTER.branch_card_code_id = INTERX.branch_card_code_id 
			inner join issuer on issuer.issuer_id=INTERX.issuer_id
	WHERE INTERX.issuer_id = COALESCE(@issuer_id, INTERX.issuer_id)
		AND INTERX.branch_id = COALESCE(@branch_id,  INTERX.branch_id)	
		AND INTERX.branch_code <> '' 
		AND INTER.product_id=COALESCE(@product_id, INTER.product_id)
	GROUP BY INTERX.issuer_id,  INTERX.branch_id,INTERX.branch_code,  INTERX.branch_card_code_id,INTERX.Spoil_reason_name,issuer_name,product_code,product_name,product_id
	ORDER BY issuer_name,  INTERX.branch_code, INTERX.branch_card_code_id,product_code,product_name
END