CREATE PROCEDURE [dbo].[usp_get_feestatusreport]
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

    -- Insert statements for procedure here
	if(@issuer_id = -1 or @issuer_id = 0)
	 set @issuer_id=null

	if(@branch_id  =0)
		set @branch_id = null

		if(@user_id  =0)
		set @user_id = null

OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)

		DECLARE @APPROVER TABLE (username VARCHAR(100),
                           card_id int)
		INSERT  INTO @APPROVER (username, card_id)
		SELECT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) as 'username' , card_id
					FROM [branch_card_status]
						INNER JOIN [user] 
							ON [user].[user_id] = [branch_card_status].[user_id]
					WHERE
						[branch_card_status].[branch_card_statuses_id] = 3


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
		, fee.fee_charge_status_id as card_fee_charge_status_id,card_fee_charge_status_language.langauge_text as 'CardFeeChargeStatus'
		, [status_date] as'Date'
		, ap.username as 'APPROVER USER'
		,  product_code+'-'+product_name as product,
		   [cards].product_id
	FROM 
		[branch_card_status_current]
			INNER JOIN [cards] 
				ON [branch_card_status_current].card_id = [cards].card_id
				INNER JOIN [fee_charged] 
				ON [fee_charged].fee_id = [cards].card_id
			INNER JOIN [branch] 
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = [cards].card_id
					INNER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id			
		inner join fee_charged as fee on fee.fee_id=cards.fee_id
			INNER JOIN [card_fee_charge_status]
							ON [card_fee_charge_status].[card_fee_charge_status_id] = fee.fee_charge_status_id
			INNER JOIN card_fee_charge_status_language
					ON card_fee_charge_status.card_fee_charge_status_id = card_fee_charge_status_language.card_fee_charge_status_id
			INNER JOIN [issuer] 
				ON [issuer].issuer_id = branch.issuer_id
				left join  @APPROVER as ap
				ON ap.card_id = [cards].card_id
					left join  @Operator as op
				ON op.card_id = [cards].card_id		
					INNER JOIN [issuer_product] 
						ON [cards].product_id=[issuer_product].product_id
	 WHERE
		--[branch_card_status_current].branch_card_statuses_id != 6 	
		--AND 
		[branch].issuer_id = COALESCE(@issuer_id, [branch].issuer_id)
		--Match on both the original operator or the final person to have issued the card
		AND (op.[userid] = COALESCE(@user_id, op.[userid]) OR
			  [branch_card_status_current].[user_id] = COALESCE(@user_id, [branch_card_status_current].[user_id]))
	    AND [cards].branch_id = COALESCE(@branch_id, [cards].branch_id) 
		AND card_fee_charge_status_language.language_id = @language_id 
		AND [branch_card_status_current].status_date >= @fromdate 
		AND [branch_card_status_current].status_date <= @todate
		AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
			AND [fee_charged].fee_waiver_YN = 0
	 ORDER BY
		CardFeeChargeStatus

	 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END

