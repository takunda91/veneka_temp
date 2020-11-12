CREATE NONCLUSTERED INDEX [INDEX_CARD_STATUS_DATE]
ON [dbo].[branch_card_status] ([card_id])
INCLUDE ([status_date])