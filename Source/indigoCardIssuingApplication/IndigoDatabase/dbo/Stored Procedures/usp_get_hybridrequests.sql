CREATE PROCEDURE [dbo].[usp_get_hybridrequests]	
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
	
	DECLARE @hybrid_request_statuses_id int
	SET @hybrid_request_statuses_id = 1

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
			   COUNT(hybrid_requests.request_id) as 'cardscount'
		FROM hybrid_requests 
				INNER JOIN issuer_product 
					ON hybrid_requests.product_id = issuer_product.product_id 
				INNER JOIN	 hybrid_request_status_current 
					ON hybrid_request_status_current.request_id = hybrid_requests.request_id 
				INNER JOIN card_priority 
					ON hybrid_requests.card_priority_id = card_priority.card_priority_id
				INNER JOIN card_priority_language 
					ON card_priority.card_priority_id=card_priority_language.card_priority_id
		WHERE (hybrid_requests.[card_issue_method_id] = 1)
				AND	hybrid_request_status_current.hybrid_request_statuses_id = 1 
				AND card_priority_language.language_id = @languageId
				AND issuer_id = @issuer_id 
				AND hybrid_requests.delivery_branch_id = COALESCE(@branch_id, hybrid_requests.delivery_branch_id)
		GROUP BY issuer_product.product_id,hybrid_requests.delivery_branch_id, card_priority.card_priority_id, language_text, product_name   
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY product_name ASC

	END
END








