CREATE TABLE [dbo].[dist_batch_statuses_language] (
    [dist_batch_statuses_id] INT           NOT NULL,
    [language_id]            INT           NOT NULL,
    [language_text]          VARCHAR (100) NOT NULL,
   CONSTRAINT [FK_dist_batch_statuses_language_dist_batch_statuses_id] FOREIGN KEY ([dist_batch_statuses_id]) REFERENCES [dbo].[dist_batch_statuses] ([dist_batch_statuses_id]),
   CONSTRAINT [FK_dist_batch_statuses_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_dist_batch_statuses_language] PRIMARY KEY ([dist_batch_statuses_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'dist_batch_statuses_language'