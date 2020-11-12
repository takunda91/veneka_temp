-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Search for export batches based on filter criteria
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_export_batches_paged] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@product_id int = null,
	@export_batch_statuses_id int = null,
	@batch_reference varchar(100) = null,
	@date_from datetime2 = null,
	@date_to datetime2 = null,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @date_to IS NOT NULL
	SET @date_to = DATEADD(day, 1, @date_to)

	DECLARE @StartRow INT, @EndRow INT;	Declare @UserTimezone as nvarchar(50)		
			
	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	set @UserTimezone=[dbo].[GetUserTimeZone](@audit_user_id);

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS			
			, *
	FROM( 
		SELECT [export_batch].*,
				[export_batch_statuses_language].language_text as export_batch_statuses_name,
				[export_batch_status_current].export_batch_statuses_id, 
				[export_batch_status_current].comments,
				[export_batch_status_current].status_date,
				[issuer].issuer_name,
				[issuer].issuer_code				
				
		FROM [export_batch]
			INNER JOIN [export_batch_status_current]
				ON [export_batch].export_batch_id = [export_batch_status_current].export_batch_id
			INNER JOIN [export_batch_statuses_language]
				ON [export_batch_statuses_language].export_batch_statuses_id = [export_batch_status_current].export_batch_statuses_id
					AND [export_batch_statuses_language].language_id = @language_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [export_batch].issuer_id
		WHERE [export_batch].issuer_id = COALESCE(@issuer_id, [export_batch].issuer_id)
			AND [export_batch].batch_reference = COALESCE(@batch_reference, [export_batch].batch_reference)
			AND SWITCHOFFSET([export_batch].date_created,@UserTimezone)   BETWEEN COALESCE(@date_from, SWITCHOFFSET([export_batch].date_created,@UserTimezone)) AND COALESCE(@date_to, SWITCHOFFSET([export_batch].date_created,@UserTimezone))
			AND	[export_batch_status_current].export_batch_statuses_id = COALESCE(@export_batch_statuses_id, [export_batch_status_current].export_batch_statuses_id)
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY date_created DESC
END