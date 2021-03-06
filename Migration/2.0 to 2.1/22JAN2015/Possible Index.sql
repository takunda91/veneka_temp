use [indigo_database_main_dev]
go

--DROP INDEX [IX_FK__connectio__langu__74B941B4] ON [dbo].[connection_parameter_type_language]
--go

SET ANSI_PADDING ON

go

CREATE NONCLUSTERED INDEX [_dta_index_cards_5_1842105603__K3_K17_K6_K1_2_4_7_8_9] ON [dbo].[cards]
(
	[branch_id] ASC,
	[sub_product_id] ASC,
	[card_index] ASC,
	[card_id] ASC
)
INCLUDE ( 	[product_id],
	[card_number],
	[card_issue_method_id],
	[card_priority_id],
	[card_request_reference]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE STATISTICS [_dta_stat_1842105603_17_6] ON [dbo].[cards]([sub_product_id], [card_index])
go

CREATE STATISTICS [_dta_stat_1842105603_6_1] ON [dbo].[cards]([card_index], [card_id])
go

CREATE STATISTICS [_dta_stat_1842105603_1_3_17] ON [dbo].[cards]([card_id], [branch_id], [sub_product_id])
go

CREATE STATISTICS [_dta_stat_1842105603_1_17_6] ON [dbo].[cards]([card_id], [sub_product_id], [card_index])
go

CREATE NONCLUSTERED INDEX [_dta_index_dist_batch_cards_5_430624577__K2_K3] ON [dbo].[dist_batch_cards]
(
	[card_id] ASC,
	[dist_card_status_id] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE STATISTICS [_dta_stat_430624577_3_2] ON [dbo].[dist_batch_cards]([dist_card_status_id], [card_id])
go

CREATE STATISTICS [_dta_stat_430624577_3_1_2] ON [dbo].[dist_batch_cards]([dist_card_status_id], [dist_batch_id], [card_id])
go

