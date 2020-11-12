CREATE TABLE [dbo].[auth_type] (
    [auth_type_id]   INT           NOT NULL,
    [auth_type_name] NVARCHAR (50) NOT NULL, 
    CONSTRAINT [PK_auth_type] PRIMARY KEY ([auth_type_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'auth_type'