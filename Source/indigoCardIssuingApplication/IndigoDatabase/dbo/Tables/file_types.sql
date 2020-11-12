CREATE TABLE [dbo].[file_types] (
    [file_type_id] INT          NOT NULL,
    [file_type]    VARCHAR (15) NOT NULL,
    CONSTRAINT [PK_file_types] PRIMARY KEY CLUSTERED ([file_type_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'file_types'