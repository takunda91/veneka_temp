CREATE TABLE [dbo].[export_batch_statuses] (
    [export_batch_statuses_id]   INT           NOT NULL,
    [export_batch_statuses_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_export_batch_statuses] PRIMARY KEY CLUSTERED ([export_batch_statuses_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'export_batch_statuses'