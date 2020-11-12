CREATE PROCEDURE [dbo].[usp_get_dist_batch_card_count] 
	@dist_batch_id bigint,
	@branch_id int,
	@product_id int = NULL,
	@card_issue_method_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT COUNT([cards].card_id)
	FROM [cards]
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
		INNER JOIN [dist_batch]
			ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
		INNER JOIN [dist_batch_status_current]
			ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id
	WHERE 
		[dist_batch].branch_id = COALESCE(@branch_id,[dist_batch].branch_id)
			AND [cards].product_id = COALESCE(@product_id,[cards].product_id)
			AND [dist_batch].dist_batch_id = @dist_batch_id
			--AND [dist_batch_status_current].dist_batch_statuses_id = 14
			AND ([dist_batch_cards].dist_card_status_id = 18 or [dist_batch_cards].dist_card_status_id = 2)
			AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)
END
GO

