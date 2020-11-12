CREATE TABLE [dbo].[product_fee_scheme] (
    [fee_scheme_id]     INT           IDENTITY (1, 1) NOT NULL,
    [issuer_id]         INT           NOT NULL,
    [fee_scheme_name]   VARCHAR (100) NOT NULL,
    [deleted_yn]        BIT           NOT NULL,
    [fee_accounting_id] INT           NOT NULL,
    CONSTRAINT [PK_product_fee] PRIMARY KEY CLUSTERED ([fee_scheme_id] ASC),
    CONSTRAINT [FK_product_fee_scheme_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id]),
    CONSTRAINT [FK_product_fee_scheme_product_fee_accounting] FOREIGN KEY ([fee_accounting_id]) REFERENCES [dbo].[product_fee_accounting] ([fee_accounting_id])
);

