USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_issuedcardsreport]    Script Date: 2015/04/28 04:28:00 PM ******/
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
ALTER PROCEDURE [dbo].[sp_get_issuedcardsreport]
	@isuerid int,
	@fromdate datetime,
	@todate datetime,
	@userid int = null,
	@brachcode nvarchar(50),
	@language_id int = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@userid=0)
	set @userid=null

	if(@isuerid=0)
	set @isuerid=null
    -- Insert statements for procedure here
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
		
	SELECT 
		[branch_code] as 'BranchCode'
		, CONVERT(VARCHAR,DECRYPTBYKEY(u.[username])) as 'IssuedBy'
		, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_first_name))+' '+ CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.customer_last_name)) as 'CustomerNames'
		, CONVERT(VARCHAR,DECRYPTBYKEY(customer_account.[customer_account_number])) as 'customeraccountNumber'
		, CASE WHEN [issuer].card_ref_preference = 1 
			THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
			ELSE CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) 
			END AS 'card_number'
		, cards.card_request_reference AS card_reference_number
		, [branch_card_statuses_name] as 'CardStatus'
		, [status_date] as'IssuedDate'
		, APPROVER.username as 'APPROVER USER'
		, language_text as 'Scenario'
		, [Cards].fee_charged as 'fee_Charged'
	FROM 
		branch_card_status_current
		INNER JOIN  cards  on branch_card_status_current.[card_id] = cards.[card_id]
		INNER JOIN  branch on cards.[branch_id]=branch.[branch_id]
		LEFT JOIN customer_account ON cards.card_id = customer_account.card_id 
	 --left JOIN
	 --customer_account_type ON customer_account.customer_account_type_id = customer_account_type.customer_account_type_id 
		INNER JOIN [user] u on u.[user_id] = branch_card_status_current.[operator_user_id]
		INNER JOIN branch_card_statuses bs on bs.[branch_card_statuses_id] =branch_card_status_current.[branch_card_statuses_id]
		INNER JOIN [card_issue_reason] on customer_account.card_issue_reason_id = [card_issue_reason].card_issue_reason_id
		INNER JOIN [card_issue_reason_language] ON [card_issue_reason].[card_issue_reason_id] = [card_issue_reason_language].[card_issue_reason_id]
		INNER JOIN (SELECT CONVERT(VARCHAR,DECRYPTBYKEY([user].username))as 'username' , card_id
					  FROM 
						branch_card_status
						INNER JOIN [user] on [user].user_id=branch_card_status.user_id
					  WHERE
						branch_card_status.[branch_card_statuses_id]  =3) AS APPROVER ON [cards].card_id = APPROVER.card_id
		INNER JOIN [issuer] ON [issuer].issuer_id = branch.issuer_id
	 WHERE
		  ISNULL(branch.issuer_id, null)=ISNULL(@isuerid, ISNULL(branch.issuer_id, null))
		  AND branch_card_status_current.branch_card_statuses_id = 6 
		  AND ISNULL(u.[user_id],null)=ISNULL(@userid, ISNULL(u.[user_id], null))
	      AND ISNULL(branch.branch_id, null)=ISNULL(@brachcode, ISNULL(branch.branch_id, null)) 
		  AND [card_issue_reason_language].language_id = @language_id 
		  AND branch_card_status_current.[status_date] >=@fromdate and branch_card_status_current.[status_date]<=@todate
	 ORDER BY
		CardStatus

	 CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END







