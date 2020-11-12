CREATE TABLE [dbo].[threed_secure_batch_cards]
(
	[threed_batch_id] BIGINT NOT NULL , 
    [card_id] BIGINT NOT NULL, 
    CONSTRAINT [FK_threed_secure_batch_cards_to_cards] FOREIGN KEY ([card_id]) REFERENCES [cards]([card_id]), 
    CONSTRAINT [FK_threed_secure_batch_cards_to_batch] FOREIGN KEY ([threed_batch_id]) REFERENCES [threed_secure_batch]([threed_batch_id]), 
    CONSTRAINT [PK_threed_secure_batch_cards] PRIMARY KEY ([threed_batch_id], [card_id])
)
