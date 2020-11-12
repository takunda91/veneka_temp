CREATE TABLE [dbo].[dist_batch_cards] (
    [dist_batch_id]       BIGINT NOT NULL,
    [card_id]             BIGINT NOT NULL,
    [dist_card_status_id] INT    NOT NULL,
    CONSTRAINT [PK_dist_batch_cards] PRIMARY KEY CLUSTERED ([dist_batch_id] ASC, [card_id] ASC),
    CONSTRAINT [FK_dist_batch_cards_cards] FOREIGN KEY ([card_id]) REFERENCES [dbo].[cards] ([card_id]),
    CONSTRAINT [FK_dist_batch_cards_dist_card_statuses] FOREIGN KEY ([dist_card_status_id]) REFERENCES [dbo].[dist_card_statuses] ([dist_card_status_id]),
    CONSTRAINT [FK_dist_batch_cards_distribution_batch] FOREIGN KEY ([dist_batch_id]) REFERENCES [dbo].[dist_batch] ([dist_batch_id])
);


GO
CREATE STATISTICS [_dta_stat_430624577_3_1_2]
    ON [dbo].[dist_batch_cards]([dist_card_status_id], [dist_batch_id], [card_id]);


GO
CREATE STATISTICS [_dta_stat_430624577_3_2]
    ON [dbo].[dist_batch_cards]([dist_card_status_id], [card_id]);

