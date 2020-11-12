CREATE TABLE [dbo].[dist_card_statuses] (
    [dist_card_status_id]   INT          NOT NULL,
    [dist_card_status_name] VARCHAR (30) NOT NULL,
    CONSTRAINT [PK_dist_card_statuses] PRIMARY KEY CLUSTERED ([dist_card_status_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'dist_card_statuses'