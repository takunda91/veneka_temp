CREATE TABLE [dbo].[print_batch_status_audit]
(  [print_batch_status_id] [bigint] IDENTITY(1,1) NOT NULL,
	[print_batch_id] [bigint] NULL,
	[print_batch_statuses_id] [int] NULL,
	[user_id] [bigint] NULL,
	[status_date] [varchar](150) NULL, 
    CONSTRAINT [PK_print_batch_status_audit] PRIMARY KEY ([print_batch_status_id])
)
