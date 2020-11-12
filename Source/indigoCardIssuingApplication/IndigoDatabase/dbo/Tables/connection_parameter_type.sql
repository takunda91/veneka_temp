CREATE TABLE [dbo].[connection_parameter_type] (
    [connection_parameter_type_id]   INT          NOT NULL,
    [connection_parameter_type_name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_connection_parameter_type] PRIMARY KEY CLUSTERED ([connection_parameter_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'connection_parameter_type'