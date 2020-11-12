CREATE TABLE [dbo].[product_issue_reason] (
    [product_id]           INT NOT NULL,
    [card_issue_reason_id] INT NOT NULL,
    CONSTRAINT [PK_product_issue_reason] PRIMARY KEY CLUSTERED ([product_id] ASC, [card_issue_reason_id] ASC),
    CONSTRAINT [FK_product_issue_reason_card_issue_reason] FOREIGN KEY ([card_issue_reason_id]) REFERENCES [dbo].[card_issue_reason] ([card_issue_reason_id]),
    CONSTRAINT [FK_product_issue_reason_issuer_product] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id])
);

