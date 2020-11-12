CREATE PROCEDURE [dbo].[usp_get_issuedcardsreport_issuedlifecycle]
	@isuerid int,
	@fromdate datetimeoffset,
	@todate datetimeoffset,
	@userid int = null,
	@branchid int=null,
	@product_id int=null,
	@language_id int
	,@audit_user_id bigint
	,@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@userid=0)
	set @userid=null

	if(@isuerid=0 or @isuerid=-1)
	set @isuerid=null

	SET @todate = DATEADD(DD, 1, @todate)
 -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

		DECLARE @APPROVER TABLE (username VARCHAR(100),
                           card_id int)
		INSERT  INTO @APPROVER (username, card_id)
		SELECT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username' , card_id
					FROM [branch_card_status_current]
						INNER JOIN [user] 
							ON [user].[user_id] = [branch_card_status_current].[user_id]
					WHERE
						[branch_card_status_current].[branch_card_statuses_id] = 6

DECLARE @cards_issued TABLE
(
	card_id bigint PRIMARY KEY,
	status_date datetimeoffset(7) not null,
	branch_id int not null,
	[user_id] bigint not null,
	branch_card_statuses_id int
)

INSERT INTO @cards_issued (card_id, status_date, branch_id, [user_id],branch_card_statuses_id)
SELECT [branch_card_status].card_id, status_date, branch_id, [user_id],branch_card_statuses_id
FROM [branch_card_status]
WHERE [branch_card_status].branch_card_statuses_id = 6
	AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone)as datetime) >= @fromdate 
		AND CAST(SWITCHOFFSET([branch_card_status].status_date,@UserTimezone) AS datetime) <= @todate
UNION
SELECT [branch_card_status_audit].card_id, status_date, branch_id, [user_id],branch_card_statuses_id
FROM [branch_card_status_audit]
WHERE [branch_card_status_audit].branch_card_statuses_id = 6
	AND CAST(SWITCHOFFSET([branch_card_status_audit].status_date,@UserTimezone)as datetime) >= @fromdate 
		AND CAST(SWITCHOFFSET([branch_card_status_audit].status_date,@UserTimezone) AS datetime) <= @todate

   

		SELECT distinct
		branch.branch_code+'-'+ branch.branch_name as 'BranchCode'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([operator].[username])) as 'IssuedBy'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
		, CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, [operator].[user_id] as original_operator_user_id
		, card_issued.[user_id]
		, cards.card_request_reference AS card_reference_number
		, [branch_card_statuses_name] as 'CardStatus'
		,CAST(SWITCHOFFSET([status_date],@UserTimezone) as datetime)   as'IssuedDate'
		--, ap.username as 'APPROVER USER'
		,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username))  as 'APPROVER USER'
		, language_text as 'Scenario'
		, fee_charged.fee_charged as 'fee_Charged'
		,issuer_product.product_code+'-'+issuer_product.product_name as 'product'
		,issuer_product.product_id
	FROM 
		@cards_issued as card_issued
			INNER JOIN [cards] 
				ON card_issued.card_id = [cards].card_id
					INNER JOIN fee_charged ON fee_charged.fee_id=cards.fee_id
			INNER JOIN [branch] 
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [customer_account_cards] ON 
				[cards].card_id = [customer_account_cards].card_id
			INNER JOIN [customer_account] ON 
			[customer_account].customer_account_id = [customer_account_cards].customer_account_id

			INNER JOIN [user] as [operator]
				ON [operator].[user_id] = [customer_account].[user_id]
			INNER JOIN [branch_card_statuses] 
				ON [branch_card_statuses].branch_card_statuses_id = card_issued.branch_card_statuses_id
			INNER JOIN [card_issue_reason_language] 
				ON [card_issue_reason_language].card_issue_reason_id = [customer_account].card_issue_reason_id
			INNER JOIN [issuer] 
				ON [issuer].issuer_id = branch.issuer_id
				INNER JOIN [user] 
							ON [user].[user_id] = card_issued.user_id				
			INNER JOIN [Issuer_product] 
				ON [cards].product_id=[issuer_product].product_id
			 WHERE	
		 [issuer].issuer_id = COALESCE(@isuerid, [branch].issuer_id)
		--Match on both the original operator or the final person to have issued the card
		AND ([operator].[user_id] = COALESCE(@userid, [operator].[user_id]) OR
			  card_issued.[user_id] = COALESCE(@userid, card_issued.[user_id]))
	    AND [cards].branch_id = COALESCE(@branchid, [cards].branch_id) 
		AND [card_issue_reason_language].language_id = @language_id 
		--AND CAST(SWITCHOFFSET(card_issued.status_date,@UserTimezone)as datetime) >= @fromdate 
		--AND CAST(SWITCHOFFSET(card_issued.status_date,@UserTimezone) AS datetime) <= @todate
		 AND [issuer_product].product_id = COALESCE(@product_id, [issuer_product].product_id) 
	   
	ORDER BY
		CardStatus
	



	 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END
