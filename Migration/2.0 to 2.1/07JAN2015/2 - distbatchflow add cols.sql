USE [indigo_database_main_dev]
GO

ALTER TABLE [dist_batch_statuses_flow]
	ADD branch_card_statuses_id int null
GO

ALTER TABLE [dist_batch_statuses_flow]
	ADD reject_branch_card_statuses_id int null
GO