use [indigo_database_main_dev]
go

--DROP INDEX [_dta_index_cards_5_1842105603__K3_K17_K6_K1_2_4_7_8_9] ON [dbo].[cards]
--go

CREATE NONCLUSTERED INDEX [_dta_index_cards_5_1842105603__K1] ON [dbo].[cards]
(
	[card_id] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

--CREATE STATISTICS [_dta_stat_1842105603_6_1_3] ON [dbo].[cards]([card_index], [card_id], [branch_id])
--go

DROP INDEX [_dta_index_dist_batch_cards_5_430624577__K2_K3] ON [dbo].[dist_batch_cards]
go

--DROP INDEX [IX_FK__connectio__langu__74B941B4] ON [dbo].[connection_parameter_type_language]
--go

