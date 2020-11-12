CREATE TABLE [dbo].[dist_card_statuses_language] (
    [dist_card_status_id] INT           NOT NULL,
    [language_id]         INT           NOT NULL,
    [language_text]       VARCHAR (100) NOT NULL,
     CONSTRAINT [FK_dist_card_status_dist_card_status_id] FOREIGN KEY ([dist_card_status_id]) REFERENCES [dbo].[dist_card_statuses] ([dist_card_status_id]),
    CONSTRAINT [FK_dist_card_status_language]  FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_dist_card_statuses_language] PRIMARY KEY ([dist_card_status_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'dist_card_statuses_language'