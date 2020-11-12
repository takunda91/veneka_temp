CREATE TABLE [dbo].[card_issue_method] (
    [card_issue_method_id]   INT          NOT NULL,
    [card_issue_method_name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_card_issue_method] PRIMARY KEY CLUSTERED ([card_issue_method_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'card_issue_method'