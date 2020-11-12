SELECT [branch_card_status_current].card_id
FROM [branch_card_status_current]
	INNER JOIN [dist_batch_status_card_current]
		ON [branch_card_status_current].card_id = [dist_batch_status_card_current].card_id
WHERE [branch_card_status_current].branch_card_statuses_id = 0
	AND [dist_batch_status_card_current].dist_card_status_id = 18

--DELETE FROM [branch_card_status]
--WHERE card_id IN (
--SELECT [branch_card_status_current].card_id
--FROM [branch_card_status_current]
--	INNER JOIN [dist_batch_status_card_current]
--		ON [branch_card_status_current].card_id = [dist_batch_status_card_current].card_id
--WHERE [branch_card_status_current].branch_card_statuses_id = 0
--	AND [dist_batch_status_card_current].dist_card_status_id = 18)