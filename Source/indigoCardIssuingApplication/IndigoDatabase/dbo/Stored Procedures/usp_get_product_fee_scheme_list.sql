-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_scheme_list] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY fee_scheme_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT [product_fee_scheme].*, [issuer].issuer_name
		FROM [product_fee_scheme]
			INNER JOIN [issuer]	
				ON [product_fee_scheme].issuer_id = [issuer].issuer_id
		WHERE [product_fee_scheme].issuer_id = COALESCE(@issuer_id, [product_fee_scheme].issuer_id)
	)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY fee_scheme_name ASC

END