CREATE TYPE [dbo].[load_cards_type] AS TABLE (
    [card_number]          VARCHAR (20)  NULL,
    [card_reference]       VARCHAR (100) NULL,
    [branch_id]            INT           NULL,
    [card_sequence]        INT           NULL,
    [expiry_date]          DATETIME2 (7) NULL,
    [product_id]           INT           NULL,
    [card_issue_method_id] INT           NULL,
    [sub_product_id]       INT           NULL,
    [already_loaded]       BIT           NULL,
    [card_id]              BIGINT        NULL,
    [load_batch_type_id]   INT           NULL);

