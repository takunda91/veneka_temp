-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pin_mailer_reprint_requests] 
	@issuer_id int,
	@branch_id int = null,
	@languageId int,	
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @pin_mailer_reprint_status_id int
	SET @pin_mailer_reprint_status_id = 1

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY product_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(		
		SELECT issuer_product.product_id,
				issuer_product.product_name,
				COUNT(cards.card_id) as 'cardscount'
		FROM [cards]
				INNER JOIN [issuer_product] 
					ON [cards].product_id = [issuer_product].product_id 
				INNER JOIN	[pin_mailer_reprint_status_current] 
					ON [pin_mailer_reprint_status_current].card_id = [cards].card_id 
		WHERE cards.[card_issue_method_id] = 0 
				AND	[pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
				AND issuer_id = @issuer_id 
				AND cards.branch_id = COALESCE(@branch_id, cards.branch_id)
		GROUP BY issuer_product.product_id, product_name   
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC

	
END