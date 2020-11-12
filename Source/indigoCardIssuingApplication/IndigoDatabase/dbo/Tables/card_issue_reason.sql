CREATE TABLE [dbo].[card_issue_reason] (
    [card_issue_reason_id]    INT           NOT NULL,
    [card_issuer_reason_name] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_card_issue_reason] PRIMARY KEY CLUSTERED ([card_issue_reason_id] ASC)
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'card_issue_reason'