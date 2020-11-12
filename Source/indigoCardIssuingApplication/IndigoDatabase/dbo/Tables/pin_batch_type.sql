CREATE TABLE [dbo].[pin_batch_type] (
    [pin_batch_type_id]   INT           NOT NULL,
    [pin_batch_type_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_pin_batch_type] PRIMARY KEY CLUSTERED ([pin_batch_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_batch_type'