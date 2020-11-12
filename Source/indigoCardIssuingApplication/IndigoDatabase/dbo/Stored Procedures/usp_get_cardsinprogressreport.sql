-- =============================================
-- Author:		sandhya konduru
-- Create date: 08/01/2017
-- Description:	for displaying cards those are in progrees before issuing to customer
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_cardsinprogressreport]
	@isuerid int,
	@fromdate datetimeoffset,
	@todate datetimeoffset,
	@userid int = null,
	@branchid int=null,
	@language_id int = null,
	@product_id int
	,@audit_user_id bigint
	,@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@userid=0)
	set @userid=null

	if(@isuerid=0)
	set @isuerid=null

	SET @todate = DATEADD(d, 1, @todate)
 -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
		DECLARE @APPROVER TABLE (username VARCHAR(100),
                           card_id int)
		INSERT  INTO @APPROVER (username, card_id)
		SELECT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username' , card_id
					FROM [branch_card_status]
						INNER JOIN [user] 
							ON [user].[user_id] = [branch_card_status].[user_id]
					WHERE
						[branch_card_status].[branch_card_statuses_id]  between 0 AND 16


						DECLARE @Operator TABLE (username VARCHAR(100),[userid] int,
                           card_id int)
		INSERT  INTO @Operator (username,[userid], card_id)
		SELECT distinct CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username' ,[user].[user_id], card_id
					FROM [branch_card_status]
						INNER JOIN [user] 
							ON [user].[user_id] = [branch_card_status].[user_id]
					WHERE
						[branch_card_status].[branch_card_statuses_id] = 1
   
		
	SELECT 
		branch.branch_code+'-'+ branch.branch_name as 'BranchCode',
		--, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([operator].[username])) as 'IssuedBy'
		op.username as 'Operator'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
		, CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		--, [operator].[user_id] as original_operator_user_id
		, [branch_card_status_current].[user_id]
		, cards.card_request_reference AS card_reference_number
		, [branch_card_statuses_name] as 'CardStatus'
		,Cast(SWITCHOFFSET([status_date],@UserTimezone) as datetime)   as'Date'
		, ap.username as 'APPROVER USER'
		, language_text as 'Scenario'
		,issuer_product.product_code+'-'+issuer_product.product_name as product
		,issuer_product.product_id
	FROM 
		[branch_card_status_current]
			INNER JOIN [cards] 
				ON [branch_card_status_current].card_id = [cards].card_id
			INNER JOIN [branch] 
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [customer_account_cards]
						ON [cards].card_id = [customer_account_cards].card_id
					INNER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
			--INNER JOIN [user] as [operator]
			--	ON [operator].[user_id] = [customer_account].[user_id]
			INNER JOIN [branch_card_statuses] 
				ON [branch_card_statuses].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
			INNER JOIN [card_issue_reason_language] 
				ON [card_issue_reason_language].card_issue_reason_id = [customer_account].card_issue_reason_id
			INNER JOIN [issuer] 
				ON [issuer].issuer_id = branch.issuer_id
				left join  @APPROVER as ap
				ON ap.card_id = [cards].card_id
					left join  @Operator as op
				ON op.card_id = [cards].card_id	
				INNER JOIN [issuer_product] 
		ON [cards].product_id=[issuer_product].product_id	
	 WHERE
		[branch_card_status_current].branch_card_statuses_id != 6 	
		AND [branch].issuer_id = COALESCE(@isuerid, [branch].issuer_id)
		--Match on both the original operator or the final person to have issued the card
		AND (op.[userid] = COALESCE(@userid, op.[userid]) OR
			  [branch_card_status_current].[user_id] = COALESCE(@userid, [branch_card_status_current].[user_id]))
	    AND [cards].branch_id = COALESCE(@branchid, [cards].branch_id) 
		AND [card_issue_reason_language].language_id = @language_id 
		AND CAST(SWITCHOFFSET( [branch_card_status_current].status_date,@UserTimezone) as datetime)  >= @fromdate 
		AND CAST(SWITCHOFFSET( [branch_card_status_current].status_date,@UserTimezone) as datetime) <= @todate		
		AND [cards].product_id=COALESCE(@product_id, [cards].product_id)

	 ORDER BY
		CardStatus

	 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END
GO