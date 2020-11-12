-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		LTladi	
-- Create date: 20150210
-- Description: Gets a list of all the terminals
-- =============================================
CREATE PROCEDURE sp_get_terminals_list
	@languageId INT = null
	,@PageIndex INT = 1
	,@RowsPerPage INT = 20
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	( 
		SELECT ROW_NUMBER() OVER(ORDER BY [terminal_name] ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
		FROM(
				SELECT  
					DISTINCT [terminals].[terminal_id]
					, [terminals].[terminal_name]
					, [terminals].[terminal_model]
				FROM
					[terminals])
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY [terminal_name] ASC

END
GO
