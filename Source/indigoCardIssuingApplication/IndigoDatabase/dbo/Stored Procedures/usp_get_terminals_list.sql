-- =============================================
-- Author:		LTladi	
-- Create date: 20150210
-- Description: Gets a list of all the terminals
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_terminals_list]
	@languageId INT = null
	,@issuer_id INT,
	@branch_id int,
	@PageIndex INT = 1
	,@RowsPerPage INT = 20
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
		SELECT ROW_NUMBER() OVER(ORDER BY [terminal_id] ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
		FROM(
			
				SELECT  
					DISTINCT [terminals].[terminal_id]
					, [terminals].[terminal_name]
					, [terminals].[terminal_model]
					, [issuer].[issuer_name],branch.branch_name,CONVERT(varchar,DECRYPTBYKEY(device_id)) as deviceid
				FROM
					[terminals]
					INNER JOIN [branch] ON [branch].[branch_id] = [terminals].[branch_id]
					INNER JOIN [issuer] ON [issuer].[issuer_id] =  [branch].[issuer_id]
				WHERE 
					issuer.issuer_id=COALESCE(@issuer_id,issuer.issuer_id) AND [terminals].branch_id = COALESCE(@branch_id, [terminals].branch_id))

		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [terminal_Id] ASC
		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;


END