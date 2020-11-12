CREATE TABLE [dbo].[products_account_types] (
    [product_id]      INT NOT NULL,
    [account_type_id] INT NOT NULL,
    CONSTRAINT [PK_products_account_types] PRIMARY KEY CLUSTERED ([product_id] ASC, [account_type_id] ASC),
    CONSTRAINT [FK_products_account_types_customer_account_type] FOREIGN KEY ([account_type_id]) REFERENCES [dbo].[customer_account_type] ([account_type_id]),
    CONSTRAINT [FK_products_account_types_issuer_product] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id])
);

