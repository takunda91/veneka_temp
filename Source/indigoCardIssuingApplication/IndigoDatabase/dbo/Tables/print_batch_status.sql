CREATE TABLE [dbo].[print_batch_status]
(
	[print_batch_id] [bigint] NOT NULL,
	[print_batch_statuses_id] [int] NOT NULL,
	[user_id] [bigint] NULL,
	[status_date] [datetimeoffset](7) NULL,
	[status_notes] [varchar](150) NULL, 
    CONSTRAINT [FK_print_batch_status_print_batch_statuses] FOREIGN KEY ([print_batch_statuses_id]) REFERENCES [print_batch_statuses]([print_batch_statuses_id]), 
    CONSTRAINT [FK_print_batch_status_user] FOREIGN KEY ([user_id]) REFERENCES [user]([user_id]), 
    CONSTRAINT [FK_print_batch_status_print_batch] FOREIGN KEY ([print_batch_id]) REFERENCES [print_batch]([print_batch_id]), 
    CONSTRAINT [PK_print_batch_status] PRIMARY KEY ([print_batch_id]),
)
