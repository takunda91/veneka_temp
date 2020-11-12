USE [indigo_database_main_dev]
GO

ALTER TABLE [cards]
	ADD fee_reference_number varchar(100) null
GO
ALTER TABLE [cards]
	ADD fee_reversal_ref_number varchar(100) null
GO
ALTER TABLE [branch_card_status]
	ADD pin_auth_user_id bigint null