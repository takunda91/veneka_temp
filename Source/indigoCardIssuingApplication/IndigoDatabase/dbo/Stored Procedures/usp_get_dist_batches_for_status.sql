
-- =============================================
-- Author:		
-- Create date: 
-- Description:	Gets a list of distribution batches 
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batches_for_status] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@issuerId int =null,
	@dist_batch_reference VARCHAR(25) = NULL,
	@dist_batch_statuses_id int = NULL,
	@branch_id INT = NULL,
	@card_issue_method_id int = NULL,
	@distBatchTypeId int = null,
	@date_start DateTimeOffset = NULL,
	@date_end DateTimeOffset = NULL,
	@cards_for_redistribution_YN bit = 0, 
	@language_id INT,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @UserTimezone nvarchar(50) = [dbo].[GetUserTimeZone](@audit_user_id);
	
	-- Correct incoming datetimeoffsets
	IF(@date_start IS NOT NULL)
		SET @date_start = ToDateTimeOffset(@date_start, @UserTimezone)

	IF(@date_end IS NOT NULL)
		SET @date_end = ToDateTimeOffset(@date_end, @UserTimezone)

	IF(@dist_batch_reference = '' or @dist_batch_reference IS NULL)
		SET @dist_batch_reference = ''

	IF @date_end IS NOT NULL
		SET @date_end = DATEADD(DAY, 1, @date_end)

	DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	DECLARE @dist_batches TABLE
	(
		TOTAL_ROWS int,
		TOTAL_PAGES int,
		ROW_NO bigint,
		dist_batch_id bigint,
		date_created datetime,
		dist_batch_reference varchar(max),
		dist_batch_type_id int,
		branch_id int NULL,
		origin_branch_id int NULL,
		cards_count int
	);	

	WITH PAGE_ROWS
		AS
		(
		SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC, dist_batch_reference ASC) AS ROW_NO
				, COUNT(*) OVER() AS TOTAL_ROWS
				, *
		FROM( 
			SELECT DISTINCT [dist_batch].dist_batch_id, 
				CAST(SWITCHOFFSET([dist_batch].date_created, @UserTimezone) as DATETIME) AS date_created, [dist_batch].dist_batch_reference, dist_batch_type_id, [dist_batch].branch_id, [dist_batch].origin_branch_id,
								COUNT([dist_batch_cards].card_id) AS 'cards_count'
			FROM [dist_batch] 
				INNER JOIN [dist_batch_status_current]
					ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
				INNER JOIN [dist_batch_statuses_language]
					ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
						AND [dist_batch_statuses_language].language_id = @language_id							
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [dist_batch].issuer_id
				INNER JOIN [card_issue_method_language]
					ON [dist_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
						AND [card_issue_method_language].language_id = @language_id						
				INNER JOIN [dist_batch_cards]
					ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
				INNER JOIN [cards] 
					ON [dist_batch_cards].card_id = cards.card_id
				INNER JOIN issuer_product 
					ON cards.product_id = issuer_product.product_id
			WHERE 
				[dist_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE issuer_status_id = 0 
					AND user_role_id IN (1, 2, 4, 5, 11, 12, 13) 
					AND [user_id] = @user_id
					AND issuer_id = COALESCE(@issuerId,  issuer_id))
				AND ([dist_batch].branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) 
								AND [user_id] = @user_id
								AND [dist_batch_status_current].dist_batch_statuses_id != 8	
								--AND [dist_batch_cards].[dist_card_status_id] = 2								
								AND branch_id = COALESCE(@branch_id, branch_id)
								AND branch_status_id = 0)
						OR [dist_batch].origin_branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (2, 4) 
								AND [dist_batch_status_current].dist_batch_statuses_id = 8
								--AND [dist_batch_cards].[dist_card_status_id] = 7
								AND [user_id] = @user_id
								AND [dist_batch].origin_branch_id = COALESCE(@branch_id, branch_id)
								AND branch_status_id = 0)
						OR [dist_batch].branch_id IS NULL)

				AND (@dist_batch_reference IS NULL OR [dist_batch].dist_batch_reference like '%' + @dist_batch_reference + '%')
				AND [dist_batch_status_current].dist_batch_statuses_id = COALESCE(@dist_batch_statuses_id, [dist_batch_status_current].dist_batch_statuses_id)
				AND [dist_batch].card_issue_method_id = COALESCE(@card_issue_method_id, [dist_batch].card_issue_method_id)
				AND [dist_batch].dist_batch_type_id = COALESCE(@distBatchTypeId, [dist_batch].dist_batch_type_id)
				AND [dist_batch].date_created >= COALESCE(@date_start, [dist_batch].date_created)
				AND [dist_batch].date_created <= COALESCE(@date_end, [dist_batch].date_created)		
			GROUP BY [dist_batch].dist_batch_id, [dist_batch].date_created, [dist_batch].dist_batch_reference, dist_batch_type_id, [dist_batch].branch_id, [dist_batch].origin_branch_id
				
		)
	AS Src )
	INSERT INTO @dist_batches (TOTAL_PAGES, TOTAL_ROWS, ROW_NO, dist_batch_id, date_created, dist_batch_reference, dist_batch_type_id, branch_id, origin_branch_id, cards_count)
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,TOTAL_ROWS, ROW_NO, dist_batch_id, date_created, dist_batch_reference, dist_batch_type_id, branch_id, origin_branch_id, cards_count
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow --and cards_count > 0
	--ORDER BY date_created DESC, dist_batch_reference ASC

	IF (@cards_for_redistribution_YN = 1)

		UPDATE t1
		SET cards_count = ISNULL(s.redist_count, 0)
		FROM @dist_batches t1
		LEFT OUTER JOIN
		(
			SELECT t2.dist_batch_id, COUNT([dist_batch_cards].card_id) as redist_count
			FROM @dist_batches t2
				INNER JOIN [dbo].[dist_batch_status_current] 
					ON [dist_batch_status_current].dist_batch_id = t2.dist_batch_id
				INNER JOIN [dbo].[dist_batch_cards] 
					ON [dist_batch_cards].[dist_batch_id] = t2.[dist_batch_id]
				INNER JOIN [branch]
					ON ([dist_batch_status_current].dist_batch_statuses_id != 8 AND [branch].branch_id = t2.branch_id)
							OR
						([dist_batch_status_current].dist_batch_statuses_id = 8 AND [branch].branch_id = t2.origin_branch_id)
				LEFT OUTER JOIN [dbo].[branch_card_status_current] 
					ON [branch_card_status_current].[card_id] = [dist_batch_cards].[card_id]
						AND [branch_card_status_current].[branch_id] = [branch].[branch_id]
			WHERE [branch].branch_id = @branch_id AND
					([branch].branch_type_id <> 0
					AND t2.dist_batch_type_id = 1
					AND [dist_batch_status_current].dist_batch_statuses_id IN (3, 8)
					AND [dist_batch_cards].[dist_card_status_id] IN (2, 7)
					AND [branch_card_status_current].[branch_card_statuses_id] = 0)
				   OR 
				   ([branch].branch_type_id = 0
					AND t2.dist_batch_type_id = 1
					AND [dist_batch_status_current].dist_batch_statuses_id = 14
					AND [dist_batch_cards].[dist_card_status_id] = 18
				   )
				   OR
				   (t2.dist_batch_type_id = 0
					AND [dist_batch_status_current].dist_batch_statuses_id = 14
					AND [dist_batch_cards].[dist_card_status_id] = 18
				   )
			GROUP BY t2.dist_batch_id
		) as s
		ON s.dist_batch_id = t1.dist_batch_id


	SELECT * FROM @dist_batches

END

