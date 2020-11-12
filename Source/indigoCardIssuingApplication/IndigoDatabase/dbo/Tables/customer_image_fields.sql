CREATE TABLE [dbo].[customer_image_fields] (
    [customer_account_id] BIGINT NOT NULL,
    [product_field_id]    INT    NOT NULL,
    [value]               IMAGE  NOT NULL,
    CONSTRAINT [PK_customer_image_fields] PRIMARY KEY CLUSTERED ([customer_account_id] ASC, [product_field_id] ASC),
    CONSTRAINT [FK_customer_image_fields_customer_account] FOREIGN KEY ([customer_account_id]) REFERENCES [dbo].[customer_account] ([customer_account_id]),
    CONSTRAINT [FK_customer_image_fields_product_fields] FOREIGN KEY ([product_field_id]) REFERENCES [dbo].[product_fields] ([product_field_id])
);

