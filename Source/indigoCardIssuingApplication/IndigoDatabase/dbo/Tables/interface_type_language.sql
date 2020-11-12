CREATE TABLE [dbo].[interface_type_language] (
    [interface_type_id] INT           NOT NULL,
    [language_id]       INT           NOT NULL,
    [language_text]     VARCHAR (100) NOT NULL,
 CONSTRAINT [FK_interface_type_language_interface_type_id]  FOREIGN KEY ([interface_type_id]) REFERENCES [dbo].[interface_type] ([interface_type_id]),
  CONSTRAINT [FK_interface_type_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_interface_type_language] PRIMARY KEY ([interface_type_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'interface_type_language'