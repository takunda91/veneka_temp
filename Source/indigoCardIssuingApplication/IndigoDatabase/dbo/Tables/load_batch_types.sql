CREATE TABLE [dbo].[load_batch_types] (
    [load_batch_type_id] INT            NOT NULL,
    [load_batch_type]    NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_load_batch_types] PRIMARY KEY CLUSTERED ([load_batch_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'load_batch_types'