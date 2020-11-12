CREATE PROCEDURE [dbo].[usp_get_cmserror_report_postissuinglifecycle]
	@branch_id int = null,
	@issuer_id int = null,
	@user_id int =null,	
	@product_id int=null,
	@language_id int,
	@fromdate datetime,
	@todate datetime,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@user_id=0)
	set @user_id=null

	if(@issuer_id=0)
	set @issuer_id=null

	SET @todate = DATEADD(d, 1, @todate)
	DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

 -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

	DECLARE @cards_cms_error TABLE
	(
		card_id bigint PRIMARY KEY,
		status_date datetimeoffset(7) not null,
		branch_id int not null,
		[user_id] bigint not null,
		branch_card_statuses_id int not null,
		branch_card_code_id int not null
	)

	INSERT INTO @cards_cms_error (card_id, status_date, branch_id, [user_id], branch_card_statuses_id, branch_card_code_id)
	--SELECT [branch_card_status].card_id, status_date, branch_id, [user_id],branch_card_statuses_id, branch_card_code_id
	--FROM [branch_card_status]
	--WHERE [branch_card_status].branch_card_statuses_id = 9
	--	AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone)as datetime) >= @fromdate 
	--	AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone) AS datetime) <= @todate
	--UNION
	SELECT [branch_card_status].card_id, [branch_card_status].status_date, [branch_card_status].branch_id, [branch_card_status].[user_id],
			[branch_card_status].branch_card_statuses_id, [branch_card_status].branch_card_code_id
	FROM [branch_card_status_audit] INNER JOIN [branch_card_status]
			ON [branch_card_status].card_id = [branch_card_status_audit].card_id
	WHERE [branch_card_status_audit].branch_card_statuses_id = 6 
		  AND [branch_card_status].branch_card_statuses_id = 9
		AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone)as datetime) >= @fromdate 
		AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone) AS datetime) <= @todate
		
	SELECT distinct
		branch.branch_code+'-'+ branch.branch_name as 'BranchCode'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
		, CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, [operator].[user_id] as original_operator_user_id
		, cards_cms_error.[user_id]
		, cards.card_request_reference AS card_reference_number
		, [branch_card_statuses_name] as 'CardStatus'
		, cards_cms_error.[status_date] as'Date'		
		, language_text as 'Scenario'
		,  product_code+'-'+product_name as product,
		   [cards].product_id
	FROM 
		@cards_cms_error as cards_cms_error
			INNER JOIN [cards] 
				ON cards_cms_error.card_id = [cards].card_id
			INNER JOIN [branch] 
				ON [cards].branch_id = [branch].branch_id

			INNER JOIN [customer_account_cards] ON 
				[cards].card_id = [customer_account_cards].card_id
			INNER JOIN [customer_account] ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id

			INNER JOIN [user] as [operator]
				ON [operator].[user_id] = [customer_account].[user_id]
			INNER JOIN [branch_card_statuses] 
				ON [branch_card_statuses].branch_card_statuses_id = cards_cms_error.branch_card_statuses_id
			INNER JOIN [card_issue_reason_language] 
				ON [card_issue_reason_language].card_issue_reason_id = [customer_account].card_issue_reason_id
			INNER JOIN [issuer] 
				ON [issuer].issuer_id = branch.issuer_id
			--left join  @Operator as ap
			--	ON ap.card_id = [cards].card_id		
			INNER JOIN [Issuer_product] 
						ON [cards].product_id=[issuer_product].product_id
	 WHERE
		 [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		--Match on both the original operator or the final person to have issued the card
		AND ([operator].[user_id] = COALESCE(@user_id, [operator].[user_id]) OR
			  cards_cms_error.[user_id] = COALESCE(@user_id, cards_cms_error.[user_id]))
	    AND [cards].branch_id = COALESCE(@branch_id, [cards].branch_id) 
		AND [card_issue_reason_language].language_id = @language_id 
		--AND [branch_card_status_current].status_date >= @fromdate 
		--AND [branch_card_status_current].status_date <= @todate
		AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
	 ORDER BY
		CardStatus

	 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END

