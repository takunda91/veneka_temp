--maybe need datacompare for lookup data
INSERT INTO [interface_type] (interface_type_id, interface_type_name)
	VALUES (9, 'REMOTE_CMS')
GO

INSERT INTO [interface_type_language] (interface_type_id, language_id, language_text)
	VALUES(9, 0, 'REMOTE_CMS'),
		  (9, 1, 'REMOTE_CMS_fr'),
	      (9, 2, 'REMOTE_CMS_pt'),
	      (9, 3, 'REMOTE_CMS_es')
GO

INSERT INTO [remote_update_statuses] (remote_update_statuses_id, remote_update_statuses_name)
	VALUES (0, 'PENDING'),
			(1, 'SENT'),
			(2, 'COMPLETE'),
			(3, 'RESEND'),
			(4, 'FAILED')
GO

INSERT INTO [remote_update_statuses_language] (remote_update_statuses_id, language_id, language_text)
	VALUES (0, 0, 'PENDING'), (0, 1, 'PENDING_fr'), (0, 2, 'PENDING_pt'), (0, 3, 'PENDING_es'),
			(1, 0, 'SENT'), (1, 1, 'SENT_fr'), (1, 2, 'SENT_pt'), (1, 3, 'SENT_es'),
			(2, 0, 'COMPLETE'), (2, 1, 'COMPLETE_fr'), (2, 2, 'COMPLETE_pt'), (2, 3, 'COMPLETE_es'),
			(3, 0, 'RESEND'), (3, 1, 'RESEND_fr'), (3, 2, 'RESEND_pt'), (3, 3, 'RESEND_es'),
			(4, 0, 'FAILED'), (4, 1, 'FAILED_fr'), (4, 2, 'FAILED_pt'), (4, 3, 'FAILED_es')

GO 

INSERT INTO [external_system_types] (external_system_type_id, system_type_name)
	VALUES	(4, 'Remote CMS')
GO

INSERT INTO [external_system_types_language] (external_system_type_id, language_id, language_text)
	VALUES (4, 0, 'Remote CMS'),
			(4, 1, 'Remote CMS_fr'),
			(4, 2, 'Remote CMS_pt'),
			(4, 3, 'Remote CMS_es')
GO

INSERT INTO [card_fee_charge_status] (card_fee_charge_status_id, card_fee_charge_status_name)
VALUES (0, 'PENDING'),
		(1, 'SUCCESSFUL'),
		(2, 'FAILED'),
		(3, 'REVERSED')
GO

INSERT INTO [card_fee_charge_status_language] (card_fee_charge_status_id, language_id, langauge_text)
VALUES (0, 0, 'PENDING'), (0, 1, 'PENDING_fr'), (0, 2, 'PENDING_pt'), (0, 3, 'PENDING_es'),
		(1, 0, 'SUCCESSFUL'), (1, 1, 'SUCCESSFUL_fr'), (1, 2, 'SUCCESSFUL_pt'), (1, 3, 'SUCCESSFUL_es'),
		(2, 0, 'FAILED'), (2, 1, 'FAILED_fr'), (2, 2, 'FAILED_pt'), (2, 3, 'FAILED_es'),
		(3, 0, 'REVERSED'), (3, 1, 'REVERSED_fr'), (3, 2, 'REVERSED_pt'), (3, 3, 'REVERSED_es')