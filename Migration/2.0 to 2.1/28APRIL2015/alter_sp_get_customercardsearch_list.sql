USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_customercardsearch_list]    Script Date: 2015/04/28 04:06:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_customercardsearch_list]
	@issuer_id int =null,
	@branch_id int =null,
	@cardrefno nvarchar(50),
	@customeraccountno nvarchar(50),
	@product_id int =null,
	@card_Priority int=null,
	@card_issuemethod int =null,
		@PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		IF @customeraccountno = '' OR @customeraccountno IS NULL
		SET @customeraccountno = NULL
	ELSE
		SET @customeraccountno = '%' + @customeraccountno + '%'
			
			IF @cardrefno = '' OR @cardrefno IS NULL
		SET @cardrefno = NULL
	ELSE
		SET @cardrefno = '%' + @cardrefno + '%'

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY product_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
	
    -- Insert statements for procedure here
	SELECT
		ip.product_name 
		, CASE WHEN [issuer].card_ref_preference = 1 
			THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY(c.card_number)),6,4)
			ELSE CONVERT(VARCHAR,DECRYPTBYKEY(c.card_number)) 
			END AS 'card_number'
		, c.card_request_reference AS card_reference_number

		, ca.card_id
		, CONVERT(VARCHAR,DECRYPTBYKEY(ca.customer_first_name)) as 'first_name'
		, CONVERT(VARCHAR,DECRYPTBYKEY(ca.customer_last_name)) as 'last_name'
		, CONVERT(VARCHAR,DECRYPTBYKEY(ca.customer_account_number)) AS account_number
	FROM
		customer_account ca 
		INNER JOIN cards as c ON ca.card_id = c.card_id 
		INNER JOIN [branch_card_status_current] ON [branch_card_status_current].card_id = c.card_id	
		INNER JOIN issuer_product as ip ON c.product_id = ip.product_id
		INNER JOIN [branch] ON [branch].branch_id = c.branch_id
		INNER JOIN [issuer] ON [issuer].[issuer_id] = [branch].issuer_id
	WHERE
		ip.issuer_id =COALESCE(@issuer_id, ip.issuer_id)
		AND branch.branch_id=COALESCE(@branch_id,branch.branch_id) 
		AND (@customeraccountno IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY(ca.customer_account_number)) LIKE @customeraccountno) 
		AND (@cardrefno IS NULL OR CONVERT(VARCHAR,DECRYPTBYKEY(c.card_number)) LIKE @cardrefno)
		AND CONVERT(VARCHAR,DECRYPTBYKEY(c.card_number)) <>''
		AND [branch_card_status_current].branch_card_statuses_id = 1 
		AND c.card_issue_method_id=COALESCE(@card_issuemethod, c.card_issue_method_id) 
		AND c.card_priority_id =COALESCE(@card_Priority, c.card_priority_id)
		AND c.product_id =COALESCE(@product_id, c.product_id)
		)
						  AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END



