USE [{DATABASE_NAME}]
GO
--Updates previous status 10 to current status 16 for manually issued cards.
UPDATE [dbo].[branch_card_status]
SET [branch_card_statuses_id] = 16
WHERE [branch_card_statuses_id] = 10