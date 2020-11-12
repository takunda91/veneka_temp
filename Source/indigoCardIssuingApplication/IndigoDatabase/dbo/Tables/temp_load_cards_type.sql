CREATE TABLE [dbo].[temp_load_cards_type] (
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
    [load_batch_type_id]   INT           NULL
);
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20180820-114942]
    ON [dbo].[temp_load_cards_type]([product_id] ASC);
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20180820-125139]
    ON [dbo].[temp_load_cards_type]([card_number] ASC, [card_reference] ASC);
GO

