USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batch_card_details_paged]    Script Date: 2015/04/28 04:17:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all cards linked to distribution batch with their details
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_dist_batch_card_details_paged] 
	@dist_batch_id bigint,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @StartRow INT, @EndRow INT;	
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;


	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY card_id ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM( 

		SELECT
			CASE WHEN [issuer].card_ref_preference = 1 
				THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
				ELSE CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) 
				END AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, [cards].branch_id
			, [cards].card_id
			, [cards].card_issue_method_id
			, [cards].card_priority_id
			, [cards].card_request_reference
			, [cards].card_sequence
			, [cards].product_id
			, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_activation_date)), 109) as 'card_activation_date' 
			, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_expiry_date)), 109) as 'card_expiry_date'
			, CONVERT(DATETIME,CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_production_date)), 109) as 'card_production_date'						
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].pvv)) as 'pvv'
			, [dist_batch_cards].dist_batch_id
			, [dist_batch_cards].dist_card_status_id
			, [dist_card_statuses].dist_card_status_name
			, [customer_account].account_type_id
			, [customer_account].cms_id
			, [customer_account].contract_number
			, [customer_account].currency_id, [customer_account].customer_account_id
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_account_number)) as 'customer_account_number'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].Id_number)) as 'Id_number'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([customer_account].CustomerId)) as 'CustomerId'
			, [customer_account].customer_title_id
			, [customer_account].customer_type_id
			, [customer_account].date_issued
			, [customer_account].resident_id
			, [customer_account].[user_id]
			, [issuer].issuer_id
			, [issuer].issuer_code
			, [issuer].issuer_name
			, [branch].branch_code
			, [branch].branch_name
			, [issuer_product].[product_code]
			, [issuer_product].[product_name]
			, [issuer_product].[product_bin_code]
			, [issuer_product].[src1_id]
			, [issuer_product].[src2_id]
			, [issuer_product].[src3_id]
			, [issuer_product].sub_product_id_length
			, CONVERT(INT, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVKI]))) as 'PVKI'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[PVK])) as 'PVK'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKA])) as 'CVKA'
			, CONVERT(VARCHAR(max),DECRYPTBYKEY([issuer_product].[CVKB])) as 'CVKB'
			, [issuer_product].[expiry_months]
			, [sub_product].sub_product_id
			, [sub_product].sub_product_name
			, [sub_product].sub_product_code
		FROM [cards]
			INNER JOIN [dist_batch_cards] 
				ON [dist_batch_cards].card_id = [cards].card_id	
			INNER JOIN [dist_card_statuses]
				ON 	[dist_batch_cards].dist_card_status_id = [dist_card_statuses].dist_card_status_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
			INNER JOIN [issuer]
				ON [branch].issuer_id = [issuer].issuer_id
			INNER JOIN [issuer_product]
				ON [cards].product_id = [issuer_product].product_id
			LEFT OUTER JOIN [customer_account]
				ON [cards].card_id = [customer_account].card_id
			LEFT OUTER JOIN [sub_product]
				ON [cards].sub_product_id = [sub_product].sub_product_id
					AND [cards].product_id = [sub_product].product_id
		WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id

	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY card_id ASC
														   
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		

END








