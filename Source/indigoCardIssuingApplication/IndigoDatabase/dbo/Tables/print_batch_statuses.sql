CREATE TABLE [dbo].[print_batch_statuses]
(
	[print_batch_statuses_id] [int] NOT NULL,
	[print_batch_statuses] [varchar](150) NULL, 
    CONSTRAINT [PK_print_batch_statuses] PRIMARY KEY ([print_batch_statuses_id]),
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_batch_statuses'