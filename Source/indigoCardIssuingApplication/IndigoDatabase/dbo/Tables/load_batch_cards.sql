CREATE TABLE [dbo].[load_batch_cards] (
    [load_batch_id]       BIGINT NOT NULL,
    [card_id]             BIGINT NOT NULL,
    [load_card_status_id] INT    NOT NULL,
    CONSTRAINT [PK_load_batch_cards] PRIMARY KEY CLUSTERED ([load_batch_id] ASC, [card_id] ASC),
    CONSTRAINT [FK_load_batch_cards_cards] FOREIGN KEY ([card_id]) REFERENCES [dbo].[cards] ([card_id]),
    CONSTRAINT [FK_load_batch_cards_load_batch] FOREIGN KEY ([load_batch_id]) REFERENCES [dbo].[load_batch] ([load_batch_id]),
    CONSTRAINT [FK_load_batch_cards_load_card_statuses] FOREIGN KEY ([load_card_status_id]) REFERENCES [dbo].[load_card_statuses] ([load_card_status_id])
);

