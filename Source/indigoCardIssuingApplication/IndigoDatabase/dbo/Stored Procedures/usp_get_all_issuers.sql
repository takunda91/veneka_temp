-- =============================================
-- Author:		Selebalo Setenane
-- Create date: 2014/03/19
-- Description:	Returns all the issuers and their interfaces
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_all_issuers]
	 @languageId int =null,
	 @PageIndex INT = 1,
	@RowsPerPage INT = 20
AS
BEGIN
	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY issuer_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
	SELECT  distinct      issuer.issuer_id, issuer.issuer_name, issuer.issuer_code, issuer_statuses_language.language_text as 'issuer_status'
FROM            issuer INNER JOIN
                issuer_statuses 
				ON issuer.issuer_status_id = issuer_statuses.issuer_status_id 
				INNER JOIN
                issuer_statuses_language ON 
			    issuer_statuses.issuer_status_id = issuer_statuses_language.issuer_status_id
				where issuer_statuses_language.[language_id]=@languageId
					 )
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY issuer_name ASC
	--SELECT 
	--[dbo].[issuer_id] AS [issuer_id], 
	--[dbo].[issuer_name] AS [issuer_name], 
	--[dbo].[issuer_code] AS [issuer_code], 
	--[dbo].[issuer_status] AS [issuer_status], 
	--[dbo].[license_file] AS [license_file], 
	--[dbo].[cards_file_location] AS [cards_file_location], 
	--[dbo].[pins_file_location] AS [pins_file_location], 
	--[dbo].[encrypted_ZPK] AS [encrypted_ZPK], 
	--[dbo].[instant_card_issue_YN] AS [instant_card_issue_YN], 
	--[dbo].[pin_mailer_printing_YN] AS [pin_mailer_printing_YN], 
	--[dbo].[delete_pin_file_YN] AS [delete_pin_file_YN], 
	--[dbo].[delete_card_file_YN] AS [delete_card_file_YN], 
	--[dbo].[pin_mailer_file_type] AS [pin_mailer_file_type], 
	--[dbo].[card_file_type] AS [card_file_type], 
	--[dbo].[pin_printer_name] AS [pin_printer_name], 
	--[dbo].[encrypted_PWK] AS [encrypted_PWK], 
	--[dbo].[C1] AS [C1], 
	--[dbo].[id] AS [id], 
	--[dbo].[interface_type_id] AS [interface_type_id], 
	--[dbo].[issuer_issuer_id] AS [issuer_issuer_id]
	--FROM ( SELECT 
	--	[dbo].[issuer_id] AS [issuer_id], 
	--	[dbo].[issuer_name] AS [issuer_name], 
	--	[dbo].[issuer_code] AS [issuer_code], 
	--	[dbo].[issuer_status] AS [issuer_status], 
	--	[dbo].[license_file] AS [license_file], 
	--	[dbo].[cards_file_location] AS [cards_file_location], 
	--	[dbo].[pins_file_location] AS [pins_file_location], 
	--	[dbo].[encrypted_ZPK] AS [encrypted_ZPK], 
	--	[dbo].[instant_card_issue_YN] AS [instant_card_issue_YN], 
	--	[dbo].[pin_mailer_printing_YN] AS [pin_mailer_printing_YN], 
	--	[dbo].[delete_pin_file_YN] AS [delete_pin_file_YN], 
	--	[dbo].[delete_card_file_YN] AS [delete_card_file_YN], 
	--	[dbo].[pin_mailer_file_type] AS [pin_mailer_file_type], 
	--	[dbo].[card_file_type] AS [card_file_type], 
	--	[dbo].[pin_printer_name] AS [pin_printer_name], 
	--	[dbo].[encrypted_PWK] AS [encrypted_PWK], 
	--	[Extent2].[id] AS [id], 
	--	[Extent2].[interface_type_id] AS [interface_type_id], 
	--	[Extent2].[issuer_issuer_id] AS [issuer_issuer_id], 
	--	CASE WHEN ([Extent2].[id] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C1]
	--	FROM  [dbo].[issuer] AS [dbo]
	--	LEFT OUTER JOIN [dbo].[interface] AS [Extent2] ON [dbo].[issuer_id] = [Extent2].[issuer_issuer_id]
	--)  AS [dbo]
	--ORDER BY [dbo].[issuer_id] ASC, [dbo].[C1] ASC
	END