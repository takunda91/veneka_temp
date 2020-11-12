Create PROCEDURE [dbo].[usp_get_auth_configuration_list]	
	@PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN


	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY authentication_configuration ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(	

	SELECT        authentication_configuration_id, authentication_configuration
FROM            auth_configuration


)

		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY authentication_configuration ASC

	

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END