CREATE VIEW [dbo].[print_batch_status_current]
	AS SELECT        print_batch_id, print_batch_statuses_id, user_id, status_date, status_notes
FROM            dbo.print_batch_status
