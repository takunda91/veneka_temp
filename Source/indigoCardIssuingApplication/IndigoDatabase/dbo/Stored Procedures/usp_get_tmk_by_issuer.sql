-- =============================================
-- Author:		LTladi	
-- Create date: 20150213
-- Description:	Get Masterkeys for issuer
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_tmk_by_issuer]
	@issuer_id INT
	, @branch_id INT=null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN

	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY [masterkey_name] DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
		SELECT 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) as 'masterkey'
			, [masterkeys].[masterkey_name]
			, [masterkeys].issuer_id
			, [issuer].issuer_name
			, [masterkeys].masterkey_id
		FROM [masterkeys]
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [masterkeys].issuer_id
						AND [issuer].issuer_status_id = 0
		WHERE [masterkeys].issuer_id = @issuer_id
			)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [masterkey_name] DESC
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END