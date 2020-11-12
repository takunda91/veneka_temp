CREATE TABLE [dbo].[pin_reissue_statuses_language] (
    [pin_reissue_statuses_id] INT           NOT NULL,
    [language_id]             INT           NOT NULL,
    [language_text]           VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_pin_reissue_statuses_language] PRIMARY KEY CLUSTERED ([pin_reissue_statuses_id] ASC, [language_id] ASC),
    CONSTRAINT [FK_pin_reissue_statuses_language_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
    CONSTRAINT [FK_pin_reissue_statuses_language_pin_reissue_statuses] FOREIGN KEY ([pin_reissue_statuses_id]) REFERENCES [dbo].[pin_reissue_statuses] ([pin_reissue_statuses_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_reissue_statuses_language'