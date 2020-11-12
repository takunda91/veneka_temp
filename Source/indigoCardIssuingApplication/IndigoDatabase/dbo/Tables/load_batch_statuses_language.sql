CREATE TABLE [dbo].[load_batch_statuses_language] (
    [load_batch_statuses_id] INT           NOT NULL,
    [language_id]            INT           NOT NULL,
    [language_text]          VARCHAR (100) NOT NULL,
   CONSTRAINT [FK_load_batch_statuses_language_id]  FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
    CONSTRAINT [FK_load_batch_statuses_load_batch_statuses_id]  FOREIGN KEY ([load_batch_statuses_id]) REFERENCES [dbo].[load_batch_statuses] ([load_batch_statuses_id]), 
    CONSTRAINT [PK_load_batch_statuses_language] PRIMARY KEY ([load_batch_statuses_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'load_batch_statuses_language'