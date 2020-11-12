CREATE PROCEDURE [dbo].[usp_get_spoilcardsreport_issuedlifecycle]
	@isuerid int = null,
	@language_id int,
	@userid int = null,
	@branchid int = null,
	@fromdate datetimeoffset,
	@todate datetimeoffset,
	@product_id int=null,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@userid = 0)
		set @userid = null

	if(@isuerid = 0)
		set @isuerid=null

	if(@branchid = 0)
		set @branchid = null

		SET @todate = DATEADD(DD, 1, @todate)

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)
	
	DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

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

    -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
		SELECT 
			DISTINCT branch.branch_name+'-'+ branch_code  as 'BranchCode'
			, CONVERT(VARCHAR,DECRYPTBYKEY(u.[username])) as 'SpoliedBy'
			,'' as 'IssuedBy'
			, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
			, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
			,CASE 
				WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
				ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
			END AS 'cardnumber'
			, cards.card_request_reference AS cardreferencnumber
			, [branch_card_statuses_name] as 'CardStatus'
			, CAST(SWITCHOFFSET(cards_spoilt.[status_date],@UserTimezone) as datetime) as'IssuedDate'
			, bcl.[language_text] as 'Reason'
			,issuer_product.product_code+'-'+issuer_product.product_name as product
			,issuer_product.product_id
		FROM @cards_spoilt as cards_spoilt
				INNER JOIN cards  
					ON cards_spoilt.[card_id] = cards.[card_id]
				INNER JOIN branch 
					ON cards.[branch_id] = branch.[branch_id]
				INNER JOIN issuer 
					ON issuer.issuer_id = branch.issuer_id
				LEFT JOIN [customer_account_cards] 
					ON cards.card_id = [customer_account_cards].card_id 
			INNER JOIN [customer_account] 
			ON [customer_account].customer_account_id = [customer_account_cards].customer_account_id

				INNER JOIN [user] u 
					ON u.[user_id] = cards_spoilt.[user_id]
				INNER JOIN branch_card_statuses bs 
					ON bs.[branch_card_statuses_id] = cards_spoilt.[branch_card_statuses_id] 
				INNER JOIN [branch_card_codes_language] bcl 
					ON cards_spoilt.[branch_card_code_id] = bcl.[branch_card_code_id]
		 
				--left JOIN 
				--(SELECT [branch_card_status].operator_user_id, [branch_card_status].card_id
				-- FROM [branch_card_status]
				-- WHERE [branch_card_status].branch_card_statuses_id = 2) AS Operator
					--ON Operator.card_id = cards.[card_id]
				INNER JOIN [Issuer_product] 
					ON [cards].product_id=[issuer_product].product_id

		WHERE branch.issuer_id = COALESCE(@isuerid, branch.issuer_id)
				AND u.user_id = COALESCE(@userid, u.user_id)
				AND branch.branch_id = COALESCE(@branchid, branch.branch_id)
				AND bcl.[language_id]=@language_id
				--AND CAST(SWITCHOFFSET(branch_card_status_current.[status_date],@UserTimezone) as datetime)  >= @fromdate 
				--AND CAST(SWITCHOFFSET(branch_card_status_current.[status_date],@UserTimezone) as datetime) <= @todate 
				--AND is_exception = 1
				AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
								
			
		ORDER BY CardStatus
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END