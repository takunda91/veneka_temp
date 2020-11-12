
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_accounting_list] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY [fee_accounting_name] ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT [fee_accounting_id]
		  ,[fee_accounting_name]
		  ,[issuer_id]      
		  ,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([fee_revenue_account_no])) as [fee_revenue_account_no]
		  ,[fee_revenue_account_type_id]
		  ,[fee_revenue_branch_code]
		  ,[fee_revenue_narration_en]
		  ,[fee_revenue_narration_fr]
		  ,[fee_revenue_narration_pt]
		  ,[fee_revenue_narration_es]
		  ,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([vat_account_no])) as [vat_account_no]
		  ,[vat_account_type_id]
		  ,[vat_account_branch_code]
		  ,[vat_narration_en]
		  ,[vat_narration_fr]
		  ,[vat_narration_pt]
		  ,[vat_narration_es]
		FROM [product_fee_accounting]
		WHERE issuer_id = COALESCE(@issuer_id, issuer_id)
	)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [fee_accounting_name] ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END