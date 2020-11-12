USE [indigo_database_main_dev]
GO

/****** Object:  Index [_dta_index_cards_5_1842105603__K3_K17_K6_K1_2_4_7_8_9]    Script Date: 2015-08-06 10:22:17 AM ******/
CREATE NONCLUSTERED INDEX [_dta_index_cards_5_1842105603__K3_K17_K6_K1_2_4_7_8_9] ON [dbo].[cards]
(
	[branch_id] ASC,	
	[card_index] ASC,
	[card_id] ASC
)
INCLUDE ( 	[product_id],
	[card_number],
	[card_issue_method_id],
	[card_priority_id],
	[card_request_reference]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


