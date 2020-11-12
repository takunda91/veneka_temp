CREATE TABLE [dbo].[pin_mailer_reprint_statuses_language] (
    [pin_mailer_reprint_status_id] INT            NOT NULL,
    [language_id]                  INT            NOT NULL,
    [language_text]                NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_pin_mailer_reprint_statuses_language] PRIMARY KEY CLUSTERED ([pin_mailer_reprint_status_id] ASC, [language_id] ASC),
    CONSTRAINT [FK_pin_mailer_reprint_statuses_language_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]),
    CONSTRAINT [FK_pin_mailer_reprint_statuses_language_pin_mailer_reprint_statuses] FOREIGN KEY ([pin_mailer_reprint_status_id]) REFERENCES [dbo].[pin_mailer_reprint_statuses] ([pin_mailer_reprint_status_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_mailer_reprint_statuses_language'