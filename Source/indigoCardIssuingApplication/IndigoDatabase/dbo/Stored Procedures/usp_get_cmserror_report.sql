-- =============================================
-- Author:		sandhya konduru
-- Create date: 22/02/2017
-- Description:	for displaying cms error cards
-- =============================================
----exec usp_get_cmserrorreport 2,'6/14/2014 12:00:00 AM','6/27/2017 12:00:00 AM',null,null,0,25,null
CREATE PROCEDURE [dbo].[usp_get_cmserror_report]
	@branch_id int = null,
	@issuer_id int = null,
	@user_id int =null,
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
 -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

		
			DECLARE @Operator TABLE (username VARCHAR(100),[userid] int,
                           card_id int)
		INSERT  INTO @Operator (username,[userid], card_id)
		SELECT distinct CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username' ,[user].[user_id], card_id
					FROM [branch_card_status]
						INNER JOIN [user] 
							ON [user].[user_id] = [branch_card_status].[user_id]
					WHERE
						[branch_card_status].[branch_card_statuses_id] = 9

   
		
	SELECT 
		branch.branch_code+'-'+ branch.branch_name as 'BranchCode'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
		, CASE 
			WHEN @mask_report = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		  END AS 'card_number'
		, [operator].[user_id] as original_operator_user_id
		, [branch_card_status_current].[user_id]
		, cards.card_request_reference AS card_reference_number
		, [branch_card_statuses_name] as 'CardStatus'
		, [status_date] as'Date'
		, ap.username as 'Operator'
		, language_text as 'Scenario'
	FROM 
		[branch_card_status_current]
			INNER JOIN [cards] 
				ON [branch_card_status_current].card_id = [cards].card_id
			INNER JOIN [branch] 
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = [cards].card_id
					INNER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
			INNER JOIN [user] as [operator]
				ON [operator].[user_id] = [customer_account].[user_id]
			INNER JOIN [branch_card_statuses] 
				ON [branch_card_statuses].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
			INNER JOIN [card_issue_reason_language] 
				ON [card_issue_reason_language].card_issue_reason_id = [customer_account].card_issue_reason_id
			INNER JOIN [issuer] 
				ON [issuer].issuer_id = branch.issuer_id
				left join  @Operator as ap
				ON ap.card_id = [cards].card_id		
	 WHERE
		[branch_card_status_current].branch_card_statuses_id = 9	
		AND [branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		--Match on both the original operator or the final person to have issued the card
		AND ([operator].[user_id] = COALESCE(@user_id, [operator].[user_id]) OR
			  [branch_card_status_current].[user_id] = COALESCE(@user_id, [branch_card_status_current].[user_id]))
	    AND [cards].branch_id = COALESCE(@branch_id, [cards].branch_id) 
		AND [card_issue_reason_language].language_id = @language_id 
		AND [branch_card_status_current].status_date >= @fromdate 
		AND [branch_card_status_current].status_date <= @todate
	 ORDER BY
		CardStatus

	 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END









