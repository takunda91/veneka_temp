CREATE TABLE [dbo].[branch_statuses] (
    [branch_status_id] INT          NOT NULL,
    [branch_status]    VARCHAR (15) NOT NULL,
    CONSTRAINT [PK_branch_status] PRIMARY KEY CLUSTERED ([branch_status_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_statuses'