CREATE TABLE [dbo].[file_statuses] (
    [file_status_id] INT          NOT NULL,
    [file_status]    VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_file_statuses] PRIMARY KEY CLUSTERED ([file_status_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'file_statuses'