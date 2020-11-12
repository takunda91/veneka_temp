CREATE TABLE [dbo].[print_statuses_language]
(
	[print_statuses_id] INT NOT NULL,
	[language_id]      INT           NOT NULL,
    [language_text]    NVARCHAR (100) NOT NULL,
	CONSTRAINT [FK_print_statuses_language_print_status]  FOREIGN KEY ([print_statuses_id]) REFERENCES [dbo].[print_statuses] ([print_statuses_id]),
    CONSTRAINT [FK_print_statuses_language_language] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_print_statuses_language] PRIMARY KEY ([print_statuses_id], [language_id])
);
GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_statuses_language'
