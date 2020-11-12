CREATE TABLE [dbo].[customer_account_cards](
	[customer_account_id] [bigint] NOT NULL,
	[card_id] [bigint] NOT NULL,
	print_job_id bigint
 CONSTRAINT [PK_customer_account_cards] PRIMARY KEY CLUSTERED 
(
	[customer_account_id] ASC,
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[customer_account_cards]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_cards_cards] FOREIGN KEY([customer_account_id])
REFERENCES [dbo].[customer_account] ([customer_account_id])
GO

ALTER TABLE [dbo].[customer_account_cards] CHECK CONSTRAINT [FK_customer_account_cards_cards]
GO

ALTER TABLE [dbo].[customer_account_cards]  WITH CHECK ADD  CONSTRAINT [FK_customer_account_cards_customer_account] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO

ALTER TABLE [dbo].[customer_account_cards] CHECK CONSTRAINT [FK_customer_account_cards_customer_account]
GO
insert [customer_account_cards]([customer_account_id],[card_id]) 
SELECT customer_account_id,card_id
from  [dbo].customer_account
GO
ALTER TABLE [dbo].[customer_account] DROP CONSTRAINT [FK_customer_account_cards];
GO
ALTER TABLE [dbo].[customer_account] DROP CONSTRAINT  AK_card_id
GO
ALTER TABLE [dbo].[customer_account] DROP COLUMN [card_id]