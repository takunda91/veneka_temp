[{DATABASE_NAME}].[dbo].[load_batch_cards]
SELECT [load_batch_id], [card_id], [load_card_status_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[load_batch_cards]