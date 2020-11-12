CREATE TABLE [dbo].[print_batch_statuses_language]
(
[print_batch_statuses_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](150) NULL, 
    CONSTRAINT [PK_print_batch_statuses_language] PRIMARY KEY ([language_id], [print_batch_statuses_id]), 
    CONSTRAINT [FK_print_batch_statuses_language_print_batch] FOREIGN KEY ([print_batch_statuses_id]) REFERENCES [print_batch_statuses]([print_batch_statuses_id]), 
    CONSTRAINT [[FK_print_batch_statuses_language_language]]] FOREIGN KEY (language_id) REFERENCES [languages](id),
)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_batch_statuses_language'