CREATE TABLE [dbo].[pin_batch_cards] (
    [pin_batch_id]                BIGINT NOT NULL,
    [card_id]                     BIGINT NOT NULL,
    [pin_batch_cards_statuses_id] INT    NOT NULL,
    CONSTRAINT [PK_pin_batch_cards] PRIMARY KEY CLUSTERED ([pin_batch_id] ASC, [card_id] ASC),
    CONSTRAINT [FK_pin_batch_cards_cards] FOREIGN KEY ([card_id]) REFERENCES [dbo].[cards] ([card_id]),
    CONSTRAINT [FK_pin_batch_cards_pin_batch] FOREIGN KEY ([pin_batch_id]) REFERENCES [dbo].[pin_batch] ([pin_batch_id]),
    CONSTRAINT [FK_pin_batch_cards_pin_batch_card_statuses] FOREIGN KEY ([pin_batch_cards_statuses_id]) REFERENCES [dbo].[pin_batch_card_statuses] ([pin_batch_card_statuses_id])
);

