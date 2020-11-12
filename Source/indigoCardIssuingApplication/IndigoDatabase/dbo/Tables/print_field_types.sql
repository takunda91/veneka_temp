CREATE TABLE [dbo].[print_field_types] (
    [print_field_type_id] INT          NOT NULL,
    [print_field_name]    VARCHAR (50) NULL,
    CONSTRAINT [PK_print_field_types] PRIMARY KEY CLUSTERED ([print_field_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_field_types'