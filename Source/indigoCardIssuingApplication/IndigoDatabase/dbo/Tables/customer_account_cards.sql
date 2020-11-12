CREATE TABLE [dbo].[customer_account_cards]
(
	[customer_account_id] bigint NOT NULL,
	[card_id] bigint NOT NULL, 
	[print_job_id] bigint NULL,
    CONSTRAINT [PK_customer_account_cards] PRIMARY KEY ([customer_account_id], [card_id]), 
    CONSTRAINT [FK_customer_account_cards_print_jobs] FOREIGN KEY ([print_job_id]) REFERENCES [print_jobs]([print_job_id]), 

    CONSTRAINT [FK_customer_account_cards_cards] FOREIGN KEY ([customer_account_id]) REFERENCES [customer_account]([customer_account_id]), 
    CONSTRAINT [FK_customer_account_cards_customer_account] FOREIGN KEY ([card_id]) REFERENCES [cards]([card_id]),
)
