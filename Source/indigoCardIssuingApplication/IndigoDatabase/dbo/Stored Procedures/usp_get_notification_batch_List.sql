

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_notification_batch_List]
@issuer_id int =null,
@PageIndex INT = 1,
@RowsPerPage INT = 20
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
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
		SELECT distinct     notification_batch_messages.issuer_id, notification_batch_messages.dist_batch_type_id, notification_batch_messages.dist_batch_statuses_id,  issuer.issuer_name+'-'+ issuer.issuer_code as 'issuer_name', dist_batch_type.dist_batch_type_name, dist_batch_statuses.dist_batch_status_name,channel_id
FROM            notification_batch_messages INNER JOIN
                         dist_batch_type ON notification_batch_messages.dist_batch_type_id = dist_batch_type.dist_batch_type_id INNER JOIN
                         dist_batch_statuses ON notification_batch_messages.dist_batch_statuses_id = dist_batch_statuses.dist_batch_statuses_id INNER JOIN
                         issuer ON notification_batch_messages.issuer_id = issuer.issuer_id
						where notification_batch_messages.issuer_id=COALESCE(notification_batch_messages.issuer_id,@issuer_id)
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY issuer_name ASC
END