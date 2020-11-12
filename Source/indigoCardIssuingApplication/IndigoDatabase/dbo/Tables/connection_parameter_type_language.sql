CREATE TABLE [dbo].[connection_parameter_type_language] (
    [connection_parameter_type_id] INT            NOT NULL,
    [language_id]                  INT            NOT NULL,
    [language_text]                NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_connection_parameter_type_language] PRIMARY KEY CLUSTERED ([connection_parameter_type_id] ASC, [language_id] ASC),
    CONSTRAINT [FK_connection_parameter_type_connection_parameter_type_id] FOREIGN KEY ([connection_parameter_type_id]) REFERENCES [dbo].[connection_parameter_type] ([connection_parameter_type_id]),
    CONSTRAINT [FK_connection_parameter_type_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id])
);


GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'connection_parameter_type_language'