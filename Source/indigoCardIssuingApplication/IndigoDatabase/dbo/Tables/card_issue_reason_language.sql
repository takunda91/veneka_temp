CREATE TABLE [dbo].[card_issue_reason_language] (
    [card_issue_reason_id] INT           NOT NULL,
    [language_id]          INT           NOT NULL,
    [language_text]        VARCHAR (100) NOT NULL,
 CONSTRAINT  [FK_card_issue_reason_language_card_issue_reason_id]  FOREIGN KEY ([card_issue_reason_id]) REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id]),
  CONSTRAINT [FK_card_issue_reason_language_language_id] FOREIGN KEY ([language_id]) REFERENCES [dbo].[languages] ([id]), 
    CONSTRAINT [PK_card_issue_reason_language] PRIMARY KEY ([card_issue_reason_id], [language_id])
);

GO
EXEC sp_addextendedproperty @name = N'VENEKA_TABLE_TYPE',
    @value = N'lookup',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'card_issue_reason_language'