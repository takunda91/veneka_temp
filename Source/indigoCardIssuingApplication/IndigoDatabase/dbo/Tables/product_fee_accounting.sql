CREATE TABLE [dbo].[product_fee_accounting] (
    [fee_accounting_id]           INT             IDENTITY (1, 1) NOT NULL,
    [fee_accounting_name]         NVARCHAR (100)  NOT NULL,
    [issuer_id]                   INT             NOT NULL,
    [fee_revenue_account_no]      VARBINARY (MAX) NOT NULL,
    [fee_revenue_account_type_id] INT             NOT NULL,
    [fee_revenue_branch_code]     NVARCHAR (10)   NULL,
    [fee_revenue_narration_en]    NVARCHAR (150)  NOT NULL,
    [fee_revenue_narration_fr]    NVARCHAR (150)  NOT NULL,
    [fee_revenue_narration_pt]    NVARCHAR (150)  NOT NULL,
    [fee_revenue_narration_es]    NVARCHAR (150)  NOT NULL,
    [vat_account_no]              VARBINARY (MAX) NOT NULL,
    [vat_account_type_id]         INT             NOT NULL,
    [vat_account_branch_code]     NVARCHAR (10)   NULL,
    [vat_narration_en]            NVARCHAR (150)  NOT NULL,
    [vat_narration_fr]            NVARCHAR (150)  NOT NULL,
    [vat_narration_pt]            NVARCHAR (150)  NOT NULL,
    [vat_narration_es]            NVARCHAR (150)  NOT NULL,
    CONSTRAINT [PK_product_fee_accounting] PRIMARY KEY CLUSTERED ([fee_accounting_id] ASC),
    CONSTRAINT [FK_product_fee_accounting_customer_account_type] FOREIGN KEY ([fee_revenue_account_type_id]) REFERENCES [dbo].[customer_account_type] ([account_type_id]),
    CONSTRAINT [FK_product_fee_accounting_customer_account_type1] FOREIGN KEY ([vat_account_type_id]) REFERENCES [dbo].[customer_account_type] ([account_type_id]),
    CONSTRAINT [FK_product_fee_accounting_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id])
);

