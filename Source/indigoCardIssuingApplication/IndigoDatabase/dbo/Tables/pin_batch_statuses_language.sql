CREATE TABLE [dbo].[pin_batch_statuses_language] (
    [pin_batch_statuses_id] INT           NOT NULL,
    [language_id]           INT           NOT NULL,
    [language_text]         VARCHAR (100) NOT NULL,
    CONSTRAINT [FK_pin_batch_statuses_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
    CONSTRAINT  [FK_pin_batch_statuses_pin_batch_statuses_id] FOREIGN KEY ([pin_batch_statuses_id]) REFERENCES [dbo].[pin_batch_statuses] ([pin_batch_statuses_id]), 
    CONSTRAINT [PK_pin_batch_statuses_language] PRIMARY KEY ([pin_batch_statuses_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_batch_statuses_language'