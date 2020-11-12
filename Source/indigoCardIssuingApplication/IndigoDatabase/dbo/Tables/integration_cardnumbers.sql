CREATE TABLE [dbo].[integration_cardnumbers] (
    [card_sequence_number] VARBINARY (MAX) NOT NULL,
    [product_id]           INT             NOT NULL,
    [sub_product_id]       INT             NOT NULL,
    CONSTRAINT [PK_integration_cardnumbers] PRIMARY KEY CLUSTERED ([product_id] ASC, [sub_product_id] ASC),
    CONSTRAINT [FK_integration_cardnumbers_issuer_product] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id])
);

