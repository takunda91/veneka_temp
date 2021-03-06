USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_card_production_report]    Script Date: 2015/04/28 02:55:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_card_production_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from datetime,
	@date_to datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT DISTINCT
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) AS customer_first_name
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) AS customer_middle_name
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) AS customer_last_name
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) AS name_on_card
			, CASE WHEN [issuer].card_ref_preference = 1 
				THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
				ELSE CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) 
				END AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, [dist_batch].dist_batch_reference
			, [dist_batch].date_created
			, [issuer].issuer_name
			, [issuer].issuer_code
			, [branch].branch_name
			, branch.branch_code
		FROM [cards]
			INNER JOIN [customer_account]
				ON [cards].card_id = [customer_account].card_id
			INNER JOIN [dist_batch_cards]
				ON [cards].card_id = [dist_batch_cards].card_id
			INNER JOIN [dist_batch]
				ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
					AND [dist_batch].dist_batch_type_id = 0
			INNER JOIN [dist_batch_status]
				ON [dist_batch].dist_batch_id = [dist_batch_status].dist_batch_id
					AND [dist_batch_status].dist_batch_statuses_id = 13						
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer]
				ON [branch].issuer_id = [issuer].issuer_id	
		WHERE [cards].card_issue_method_id = 0
			AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
			AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
			AND [dist_batch].date_created >= @date_from
			AND [dist_batch].date_created <= @date_to
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
