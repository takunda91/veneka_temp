--Get records from target and check if there are any that arent in source
SELECT *
FROM [{SOURCE_DATABASE_NAME}].[dbo].[dist_batch_cards]
EXCEPT
SELECT *     
FROM [{DATABASE_NAME}].[dbo].[dist_batch_cards]