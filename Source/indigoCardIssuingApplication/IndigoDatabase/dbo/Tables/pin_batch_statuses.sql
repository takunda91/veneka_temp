CREATE TABLE [dbo].[pin_batch_statuses] (
    [pin_batch_statuses_id]   INT           NOT NULL,
    [pin_batch_statuses_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_pin_batch_statuses] PRIMARY KEY CLUSTERED ([pin_batch_statuses_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_batch_statuses'