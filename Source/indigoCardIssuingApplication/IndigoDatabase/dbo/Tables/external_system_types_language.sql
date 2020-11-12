CREATE TABLE [dbo].[external_system_types_language] (
    [external_system_type_id] INT            NOT NULL,
    [language_id]             INT            NOT NULL,
    [language_text]           VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_external_system_types_language] PRIMARY KEY CLUSTERED ([external_system_type_id] ASC, [language_id] ASC),
    CONSTRAINT [FK_external_system_types_language_external_system_types] FOREIGN KEY ([external_system_type_id]) REFERENCES [dbo].[external_system_types] ([external_system_type_id]),
    CONSTRAINT [FK_external_system_types_language_languages] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'external_system_types_language'