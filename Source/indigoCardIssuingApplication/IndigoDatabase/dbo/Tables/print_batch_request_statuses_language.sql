CREATE TABLE [dbo].[print_batch_request_statuses_language]
(
	[print_batch_request_status_id] [int] NOT NULL,
	[language_id] [int] NOT NULL,
	[language_text] [varchar](100) NULL, 
    CONSTRAINT [PK_print_batch_request_statuses_language] PRIMARY KEY ([print_batch_request_status_id], [language_id]), 
    CONSTRAINT [FK_print_batch_request_statuses_language_print_batch] FOREIGN KEY (print_batch_request_status_id) REFERENCES print_batch_request_statuses(print_batch_request_status_id),
    CONSTRAINT [FK_print_batch_request_statuses_language_languages] FOREIGN KEY ([language_id]) REFERENCES [languages]([id])

)

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_batch_request_statuses_language'