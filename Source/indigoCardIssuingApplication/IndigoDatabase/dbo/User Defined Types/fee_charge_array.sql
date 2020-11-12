CREATE TYPE [dbo].[fee_charge_array] AS TABLE (
    [currency_id]          INT             NULL,
    [fee_charge]           DECIMAL (10, 4) NULL,
    [vat]                  DECIMAL (7, 4)  NULL,
    [cbs_account_type]     VARCHAR (10)    NULL,
    [card_issue_reason_id] INT             NULL);



