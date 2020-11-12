-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	to get cards which are distributed at the branch
-- =============================================
CREATE PROCEDURE [dbo].[usp_branch_order_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetimeoffset,
	@date_to datetimeoffset,
	@product_id int=null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)
	
Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT 
		DISTINCT CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) AS customer_first_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) AS customer_middle_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) AS customer_last_name
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) AS name_on_card
		, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].contact_number)) AS contact_number
		, [dist_batch].dist_batch_reference
		,Cast(SWITCHOFFSET([dist_batch].date_created,@UserTimezone) as datetime) as date_created
		, [issuer].issuer_name
		, [issuer].issuer_code
		, [branch].branch_name
		, branch.branch_code,issuer_product.product_code+'-'+issuer_product.product_name as product,issuer_product.product_id
	FROM 
		[cards]
		INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = [cards].card_id
					INNER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
		INNER JOIN [dist_batch]
			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
				AND [dist_batch].dist_batch_type_id = 0
				AND date_created = ( SELECT MAX(db.date_created) --- to check cards are distributed or not 
												  FROM [dist_batch] db
														INNER JOIN [dist_batch_cards] dbc
															ON db.dist_batch_id = dbc.dist_batch_id
												  WHERE dbc.card_id = [cards].card_id)
		INNER JOIN [branch]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id	
		INNER JOIN [issuer_product] 
		ON [cards].product_id=[issuer_product].product_id
	WHERE 
		[cards].card_issue_method_id = 0
		AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
		AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
		AND date_created >= @date_from
		AND date_created <= @date_to
		AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
	ORDER BY 
		issuer_name
		, issuer_code
		, branch_name
		, branch_code
		, date_created
		, customer_account_number
		, customer_first_name
		, customer_last_name

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END

