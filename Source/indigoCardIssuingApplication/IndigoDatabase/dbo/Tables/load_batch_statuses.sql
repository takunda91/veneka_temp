CREATE TABLE [dbo].[load_batch_statuses] (
    [load_batch_statuses_id] INT          NOT NULL,
    [load_batch_status_name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_batch_statuses] PRIMARY KEY CLUSTERED ([load_batch_statuses_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'load_batch_statuses'