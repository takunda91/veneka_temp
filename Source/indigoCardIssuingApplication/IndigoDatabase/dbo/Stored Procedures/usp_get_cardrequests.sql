-- =============================================
-- Author:		sandhya konduru
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_cardrequests]	
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
	
	DECLARE @branch_card_statuses_id int
	SET @branch_card_statuses_id = 3

	BEGIN
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
			   card_priority.card_priority_id, 
			   card_priority_language.language_text as 'card_priority_name',
			   COUNT(cards.card_id) as 'cardscount'
		FROM cards 
				INNER JOIN issuer_product 
					ON cards.product_id = issuer_product.product_id 
				INNER JOIN	branch_card_status_current 
					ON branch_card_status_current.card_id = cards.card_id 
				INNER JOIN card_priority 
					ON cards.card_priority_id = card_priority.card_priority_id
				INNER JOIN card_priority_language 
					ON card_priority.card_priority_id=card_priority_language.card_priority_id
		WHERE (cards.[card_issue_method_id] = 0 )
				AND	branch_card_status_current.branch_card_statuses_id = 3  
				AND card_priority_language.language_id = @languageId
				AND issuer_id = @issuer_id 
				AND cards.branch_id = COALESCE(@branch_id, cards.branch_id)
		GROUP BY issuer_product.product_id,cards.delivery_branch_id, card_priority.card_priority_id, language_text, product_name   
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC

	END
END