CREATE TABLE [dbo].[file_encryption_type] (
    [file_encryption_type_id] INT           NOT NULL,
    [file_encryption_type]    VARCHAR (250) NOT NULL,
    [file_encryption_typeid]  INT           NULL,
    CONSTRAINT [PK_file_encryption_type] PRIMARY KEY CLUSTERED ([file_encryption_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'file_encryption_type'