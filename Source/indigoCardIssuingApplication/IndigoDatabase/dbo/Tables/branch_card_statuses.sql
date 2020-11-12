CREATE TABLE [dbo].[branch_card_statuses] (
    [branch_card_statuses_id]   INT          NOT NULL,
    [branch_card_statuses_name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_branch_card_statuses] PRIMARY KEY CLUSTERED ([branch_card_statuses_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'branch_card_statuses'