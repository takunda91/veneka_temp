CREATE TABLE [dbo].[export_batch_statuses_language] (
    [export_batch_statuses_id] INT           NOT NULL,
    [language_id]              INT           NOT NULL,
    [language_text]            VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_export_batch_statuses_language] PRIMARY KEY CLUSTERED ([export_batch_statuses_id] ASC, [language_id] ASC),
    CONSTRAINT [FK_export_batch_statuses_language_export_batch_statuses] FOREIGN KEY ([export_batch_statuses_id]) REFERENCES [dbo].[export_batch_statuses] ([export_batch_statuses_id]),
    CONSTRAINT [FK_export_batch_statuses_language_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'export_batch_statuses_language'