USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_spoilcardsreport]    Script Date: 2015/04/24 04:27:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		sandhya konduru
-- Create date: 16/05/2014
-- Description:	for displaying issued card report
-- =============================================
----exec sp_get_issuedcardsreport 2,'6/14/2014 12:00:00 AM','6/27/2014 12:00:00 AM' ,25,null
ALTER PROCEDURE [dbo].[sp_get_spoilcardsreport]
	@isuerid int = null,
	@language_id int,
	@userid int = null,
	@branchid int = null,
	@fromdate datetime,
	@todate datetime	

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

    -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
		SELECT 
			DISTINCT [branch_code] as 'BranchCode'
			, CONVERT(VARCHAR,DECRYPTBYKEY(u.[username])) as 'SpoliedBy'
			,'' as 'IssuedBy'
			, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
			, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
			, [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) AS 'cardnumber'
			, [branch_card_statuses_name] as 'CardStatus'
			, [status_date] as'IssuedDate'
			, bcl.[language_text] as 'Reason'
		FROM branch_card_status_current
				INNER JOIN cards  
					ON branch_card_status_current.[card_id] = cards.[card_id]
				INNER JOIN branch 
					ON cards.[branch_id] = branch.[branch_id]
				LEFT JOIN customer_account 
					ON cards.card_id = customer_account.card_id 
				INNER JOIN [user] u 
					ON u.[user_id] = branch_card_status_current.[user_id]
				INNER JOIN branch_card_statuses bs 
					ON bs.[branch_card_statuses_id] = branch_card_status_current.[branch_card_statuses_id] 
				INNER JOIN [branch_card_codes_language] bcl 
					ON branch_card_status_current.[branch_card_code_id] = bcl.[branch_card_code_id]
		  --left join (SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].username))as 'username' , card_id
		  --FROM branch_card_status 
		  --inner join [user] on [user].user_id=branch_card_status.user_id
		  --where branch_card_status.[branch_card_statuses_id]  = 6) AS Issued
		  --ON [cards].card_id = Issued.card_id
				INNER JOIN 
				(SELECT [branch_card_status].operator_user_id, [branch_card_status].card_id
				 FROM [branch_card_status]
				 WHERE [branch_card_status].branch_card_statuses_id = 2) AS Operator
					ON Operator.card_id = cards.[card_id]

		WHERE branch.issuer_id = COALESCE(@isuerid, branch.issuer_id)
				AND Operator.operator_user_id = COALESCE(@userid, Operator.operator_user_id)
				AND branch.branch_id = COALESCE(@branchid, branch.branch_id)
				AND branch_card_status_current.branch_card_statuses_id = 7  
				AND bcl.[language_id]=@language_id
				AND branch_card_status_current.[status_date] >= @fromdate 
				AND branch_card_status_current.[status_date] <= @todate 
				AND is_exception = 1
			  --ISNULL(branch.issuer_id, null) = ISNULL(@isuerid, ISNULL(branch.issuer_id, null)) and
		  --branch_card_status_current.branch_card_statuses_id = 7  and   bcl.[language_id]=@language_id
		  --and ISNULL(u.[user_id],null)=ISNULL(@userid, ISNULL(u.[user_id], null))

		  --and ISNULL(branch.branch_id, null)=ISNULL(@branchid, ISNULL(branch.branch_id, null))

		 --and branch_card_status_current.[status_date] >=@fromdate and branch_card_status_current.[status_date]<=@todate 
		--						   and  is_exception = 1
		ORDER BY CardStatus
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END







