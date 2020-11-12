CREATE TABLE [dbo].[product_currency] (
    [product_id]       INT           NOT NULL,
    [currency_id]      INT           NOT NULL,
    [is_base]          BIT           NOT NULL,
    [usr_field_name_1] VARCHAR (250) NULL,
    [usr_field_val_1]  VARCHAR (250) NULL,
    [usr_field_name_2] VARCHAR (250) NULL,
    [usr_field_val_2]  VARCHAR (250) NULL,
    CONSTRAINT [FK__product_c__curre__08162EEB] FOREIGN KEY ([currency_id]) REFERENCES [dbo].[currency] ([currency_id]),
    CONSTRAINT [FK__product_c__produ__090A5324] FOREIGN KEY ([product_id]) REFERENCES [dbo].[issuer_product] ([product_id]), 
    CONSTRAINT [PK_product_currency] PRIMARY KEY ([product_id], [currency_id])
);


GO
ALTER TABLE [dbo].[product_currency] NOCHECK CONSTRAINT [FK__product_c__curre__08162EEB];


GO
ALTER TABLE [dbo].[product_currency] NOCHECK CONSTRAINT [FK__product_c__produ__090A5324];

