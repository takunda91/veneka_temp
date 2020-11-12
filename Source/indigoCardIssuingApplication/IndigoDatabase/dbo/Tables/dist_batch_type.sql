CREATE TABLE [dbo].[dist_batch_type] (
    [dist_batch_type_id]   INT           NOT NULL,
    [dist_batch_type_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_dist_batch_type] PRIMARY KEY CLUSTERED ([dist_batch_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'dist_batch_type'