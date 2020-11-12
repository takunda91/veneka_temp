CREATE TABLE [dbo].[print_batch_request_statuses]
(
	[print_batch_request_status_id] [int] NOT NULL,
	[print_batch_request_status] [varchar](250) NULL, 
    CONSTRAINT [PK_print_batch_request_statuses] PRIMARY KEY ([print_batch_request_status_id]),
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_batch_request_statuses'