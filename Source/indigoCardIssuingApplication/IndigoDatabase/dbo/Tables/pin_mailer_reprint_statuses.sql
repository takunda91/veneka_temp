CREATE TABLE [dbo].[pin_mailer_reprint_statuses] (
    [pin_mailer_reprint_status_id]   INT           NOT NULL,
    [pin_mailer_reprint_status_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_pin_mailer_reprint_statuses] PRIMARY KEY CLUSTERED ([pin_mailer_reprint_status_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_mailer_reprint_statuses'