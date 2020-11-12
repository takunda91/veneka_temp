CREATE TABLE [dbo].[load_card_statuses] (
    [load_card_status_id] INT          NOT NULL,
    [load_card_status]    VARCHAR (15) NOT NULL,
    CONSTRAINT [PK_load_card_statuses] PRIMARY KEY CLUSTERED ([load_card_status_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'load_card_statuses'