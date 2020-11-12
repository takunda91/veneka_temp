CREATE VIEW [dbo].[remote_update_current]
	AS 
SELECT [dbo].[remote_update_status].card_id, [dbo].[remote_update_status].comments, [dbo].[remote_update_status].remote_component, 
	[dbo].[remote_update_status].remote_update_statuses_id, [dbo].[remote_update_status].status_date, [dbo].[remote_update_status].[user_id], [dbo].[remote_update_status].remote_updated_time
FROM [dbo].[remote_update_status] 
