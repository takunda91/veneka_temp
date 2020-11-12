CREATE TABLE [dbo].[print_field_types_language] (
    [print_field_type_id] INT            NOT NULL,
    [language_id]         INT            NOT NULL,
    [language_text]       VARCHAR (1000) NOT NULL,
    CONSTRAINT [FK_print_field_types_language_print_field_types] FOREIGN KEY ([print_field_type_id]) REFERENCES [dbo].[print_field_types] ([print_field_type_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'print_field_types_language'