CREATE TABLE [dbo].[customer_fields] (
    [customer_account_id] BIGINT          NOT NULL,
    [product_field_id]    INT             NOT NULL,
    [value]               VARBINARY (MAX) NULL,
    CONSTRAINT [PK_customer_fields] PRIMARY KEY CLUSTERED ([customer_account_id] ASC, [product_field_id] ASC),
    CONSTRAINT [FK_customer_fields_customer_account] FOREIGN KEY ([customer_account_id]) REFERENCES [dbo].[customer_account] ([customer_account_id]),
    CONSTRAINT [FK_customer_fields_product_fields] FOREIGN KEY ([product_field_id]) REFERENCES [dbo].[product_fields] ([product_field_id])
);

