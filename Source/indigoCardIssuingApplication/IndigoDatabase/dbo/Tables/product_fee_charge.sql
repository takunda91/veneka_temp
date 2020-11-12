CREATE TABLE [dbo].[product_fee_charge] (
    [fee_detail_id]        INT                NOT NULL,
    [currency_id]          INT                NOT NULL,
    [card_issue_reason_id] INT                NOT NULL,
    [cbs_account_type]     NCHAR (10)         NULL,
    [fee_charge]           DECIMAL (10, 4)    NOT NULL,
    [date_created]         DATETIMEOFFSET (7) NOT NULL,
    [vat]                  DECIMAL (7, 4)     CONSTRAINT [DF_product_fee_charge_vat] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [FK_product_fee_charge_currency] FOREIGN KEY ([currency_id]) REFERENCES [dbo].[currency] ([currency_id]),
    CONSTRAINT [FK_product_fee_charge_product_fee_charge] FOREIGN KEY ([fee_detail_id]) REFERENCES [dbo].[product_fee_detail] ([fee_detail_id]), 
    CONSTRAINT [PK_product_fee_charge] PRIMARY KEY ([fee_detail_id], [currency_id], [card_issue_reason_id])
);


GO
ALTER TABLE [dbo].[product_fee_charge] NOCHECK CONSTRAINT [FK_product_fee_charge_currency];


GO
ALTER TABLE [dbo].[product_fee_charge] NOCHECK CONSTRAINT [FK_product_fee_charge_product_fee_charge];




GO
ALTER TABLE [dbo].[product_fee_charge] NOCHECK CONSTRAINT [FK_product_fee_charge_currency];


GO
ALTER TABLE [dbo].[product_fee_charge] NOCHECK CONSTRAINT [FK_product_fee_charge_product_fee_charge];

