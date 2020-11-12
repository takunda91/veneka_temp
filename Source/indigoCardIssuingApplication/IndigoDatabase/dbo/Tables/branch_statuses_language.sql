CREATE TABLE [dbo].[branch_statuses_language] (
    [branch_status_id] INT           NOT NULL,
    [language_id]      INT           NOT NULL,
    [language_text]    VARCHAR (100) NOT NULL,
    CONSTRAINT [FK_branch_statuses_language_branch_status]  FOREIGN KEY ([branch_status_id]) REFERENCES [dbo].[branch_statuses] ([branch_status_id]),
    CONSTRAINT [FK_branch_statuses_language_language] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_branch_statuses_language] PRIMARY KEY ([branch_status_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_statuses_language'