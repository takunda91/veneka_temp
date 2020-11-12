CREATE TABLE [dbo].[pin_block_format] (
    [pin_block_formatid] INT           NOT NULL,
    [pin_block_format]   NVARCHAR (50) NOT NULL, 
    CONSTRAINT [PK_pin_block_format] PRIMARY KEY ([pin_block_formatid])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'pin_block_format'