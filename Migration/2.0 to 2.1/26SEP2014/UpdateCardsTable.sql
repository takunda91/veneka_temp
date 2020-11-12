USE [indigo_database_main_dev]
GO

ALTER TABLE [cards]
	ADD card_production_date varbinary(max),
		card_expiry_date varbinary(max),
		card_activation_date varbinary(max),
		pin varbinary(max),	
		pvv varbinary(max),
		cvv varbinary(max)

GO

INSERT INTO dist_card_statuses (dist_card_status_id, dist_card_status_name)
	VALUES (12, 'CREATED')

INSERT INTO dist_card_statuses (dist_card_status_id, dist_card_status_name)
	VALUES (13, 'PAN_GENERATED')

INSERT INTO dist_card_statuses (dist_card_status_id, dist_card_status_name)
	VALUES (14, 'SECURITY_GENERATED')

INSERT INTO dist_card_statuses (dist_card_status_id, dist_card_status_name)
	VALUES (15, 'PIN_MAILER_PRINTED')

INSERT INTO dist_card_statuses (dist_card_status_id, dist_card_status_name)
	VALUES (16, 'CARD_PRODUCED')
GO