CREATE TABLE [dbo].[cards](
	[card_id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [int] NOT NULL,
	[branch_id] [int] NOT NULL,
	[card_number] [varbinary](max) NOT NULL,
	[card_sequence] [int] NOT NULL,
	[card_index] [varbinary](20) NOT NULL,
	[card_issue_method_id] [int] NOT NULL,
	[card_priority_id] [int] NOT NULL,
	[card_request_reference] [varchar](100) NULL,
	[card_production_date] [varbinary](max) NULL,
	[card_expiry_date] [varbinary](max) NULL,
	[card_activation_date] [varbinary](max) NULL,
	[pvv] [varbinary](max) NULL,
	[origin_branch_id] [int] NOT NULL,
	[export_batch_id] [bigint] NULL,
	[ordering_branch_id] [int] NOT NULL,
	[delivery_branch_id] [int] NOT NULL,
	[fee_id] [bigint] NULL,
 CONSTRAINT [PK_cards] PRIMARY KEY CLUSTERED 
(
	[card_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uq_card_request_reference] UNIQUE NONCLUSTERED 
(
	[card_request_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

GO

ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_cards] FOREIGN KEY([card_id])
REFERENCES [dbo].[cards] ([card_id])
GO

ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_cards]
GO

ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_fee_charged] FOREIGN KEY([fee_id])
REFERENCES [dbo].[fee_charged] ([fee_id])
GO

ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_fee_charged]
GO



