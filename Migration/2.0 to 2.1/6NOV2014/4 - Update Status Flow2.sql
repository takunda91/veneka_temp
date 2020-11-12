USE [indigo_database_main_dev]
GO

ALTER TABLE [dist_batch_statuses_flow]
	ADD flow_dist_card_statuses_id int
GO

ALTER TABLE [dist_batch_statuses_flow]
	ADD reject_dist_card_statuses_id int