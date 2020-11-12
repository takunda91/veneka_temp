CREATE TABLE [dbo].[external_system_types] (
    [external_system_type_id] INT           NOT NULL,
    [system_type_name]        VARCHAR (150) NOT NULL,
    CONSTRAINT [PK_external_system_types] PRIMARY KEY CLUSTERED ([external_system_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'external_system_types'