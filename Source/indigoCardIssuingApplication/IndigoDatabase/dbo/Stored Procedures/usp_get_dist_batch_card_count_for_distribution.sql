CREATE PROCEDURE [dbo].[usp_get_dist_batch_card_count_for_distribution]
	@dist_batch_id bigint,
	@product_id int = NULL,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	DECLARE @is_card_center bit

	SELECT @is_card_center = [branch].branch_type_id 
	FROM [dist_batch] INNER JOIN [dist_batch_status_current]
			ON  [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
		 INNER JOIN [branch]
			ON ([dist_batch_status_current].dist_batch_statuses_id != 8 AND [branch].branch_id = [dist_batch].branch_id)
				OR
			([dist_batch_status_current].dist_batch_statuses_id = 8 AND [branch].branch_id = [dist_batch].origin_branch_id)
	WHERE [dist_batch].[dist_batch_id] = @dist_batch_id


	IF(@is_card_center = 0)
		--Available cards for distribution at a branch
		SELECT COUNT(dist_batch.dist_batch_id) as card_count
		FROM [dbo].[dist_batch]
				INNER JOIN [dbo].[dist_batch_status_current] 
					ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
				INNER JOIN [dbo].[dist_batch_cards] 
					ON [dist_batch_cards].[dist_batch_id] = [dist_batch].[dist_batch_id]
				INNER JOIN [branch]
					ON ([dist_batch_status_current].dist_batch_statuses_id != 8 AND [branch].branch_id = [dist_batch].branch_id)
						OR
					([dist_batch_status_current].dist_batch_statuses_id = 8 AND [branch].branch_id = [dist_batch].origin_branch_id)
				INNER JOIN [dbo].[branch_card_status_current] 
					ON [branch_card_status_current].[card_id] = [dist_batch_cards].[card_id]
						AND [branch_card_status_current].[branch_id] = [branch].[branch_id]
				INNER JOIN [cards]
					ON [cards].card_id = [dist_batch_cards].card_id
		WHERE [dist_batch_status_current].dist_batch_statuses_id IN (3, 8)
				AND [dist_batch_cards].[dist_card_status_id] IN (2, 7)
				AND [branch_card_status_current].[branch_card_statuses_id] = 0
				AND [dist_batch].[dist_batch_id] = @dist_batch_id
				AND [cards].product_id = COALESCE(@product_id, [cards].product_id)				

	ELSE
		--Available cards for distribution at card center
		SELECT COUNT(dist_batch.dist_batch_id) as card_count
		FROM [dbo].[dist_batch]
				INNER JOIN [dbo].[dist_batch_status_current] 
					ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
				INNER JOIN [dbo].[dist_batch_cards] 
					ON [dist_batch_cards].[dist_batch_id] = [dist_batch].[dist_batch_id]
				INNER JOIN [cards]
					ON [cards].card_id = [dist_batch_cards].card_id
		WHERE [dist_batch_status_current].dist_batch_statuses_id = 14
				AND [dist_batch_cards].[dist_card_status_id] = 18
				AND [dist_batch].[dist_batch_id] = @dist_batch_id
				AND [cards].product_id = COALESCE(@product_id, [cards].product_id)

END
