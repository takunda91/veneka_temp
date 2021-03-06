[{DATABASE_NAME}].[dbo].[branch_card_status]
SELECT b.*, dd.branch_id
FROM [{SOURCE_DATABASE_NAME}].[dbo].[branch_card_status] b
CROSS APPLY
(
SELECT TOP 1 [dist_batch_cards].card_id, branch_id, ds.status_date
FROM [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch]
		INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch_status] ds
			ON ds.dist_batch_id = [dist_batch].dist_batch_id
		INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch_cards]
			ON ds.dist_batch_id = [dist_batch_cards].dist_batch_id
WHERE ds.status_date =
        (SELECT MAX(status_date) AS Expr1
        FROM [{SOURCE_DATABASE_NAME}].[dbo].dist_batch_status AS bcs2
        WHERE dist_batch_id = ds.dist_batch_id AND status_date < b.status_date)		
	AND [dist_batch_cards].card_id = b.card_id
GROUP BY [dist_batch_cards].card_id, branch_id, ds.status_date
ORDER BY status_date DESC
) as dd
ORDER BY card_id, b.branch_card_status_id