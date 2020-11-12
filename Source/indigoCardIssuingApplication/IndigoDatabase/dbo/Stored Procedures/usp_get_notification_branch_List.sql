

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_notification_branch_List]
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
		SELECT   distinct    notification_branch_messages.issuer_id, notification_branch_messages.card_issue_method_id, notification_branch_messages.branch_card_statuses_id, issuer.issuer_name+'-'+ issuer.issuer_code as 'issuer_name',issuer.issuer_code, branch_card_statuses.branch_card_statuses_name, card_issue_method.card_issue_method_name,channel_id
FROM            notification_branch_messages INNER JOIN
                         branch_card_statuses ON notification_branch_messages.branch_card_statuses_id = branch_card_statuses.branch_card_statuses_id INNER JOIN
                         card_issue_method ON notification_branch_messages.card_issue_method_id = card_issue_method.card_issue_method_id INNER JOIN
                         issuer ON notification_branch_messages.issuer_id = issuer.issuer_id
						where notification_branch_messages.issuer_id=COALESCE(notification_branch_messages.issuer_id,@issuer_id)
		)
		AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY issuer_name ASC
END