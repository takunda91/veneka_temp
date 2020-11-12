CREATE TABLE [dbo].[product_fee_type] (
    [fee_type_id]   INT          NOT NULL,
    [fee_type_name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_product_fee_type] PRIMARY KEY CLUSTERED ([fee_type_id] ASC)
);

