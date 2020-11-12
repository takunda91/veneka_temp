CREATE TABLE [dbo].[interface_type] (
    [interface_type_id]   INT           NOT NULL,
    [interface_type_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_interface_type] PRIMARY KEY CLUSTERED ([interface_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'interface_type'