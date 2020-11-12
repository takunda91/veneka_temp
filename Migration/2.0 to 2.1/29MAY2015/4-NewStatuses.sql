USE [indigo_database_main_dev]
GO

INSERT INTO [pin_reissue_statuses] (pin_reissue_statuses_id, pin_reissue_statuses_name)
	VALUES(0, 'REQUESTED'),
			(1, 'APPROVED'),
			(2, 'REJECTED'),
			(3, 'UPLOADED'),
			(4, 'EXPIRED')

GO
INSERT INTO [pin_reissue_statuses_language] (pin_reissue_statuses_id, language_id, language_text)
	VALUES(0, 0, 'REQUESTED'),
		(0, 1, 'REQUESTED_fr'),
		(0, 2, 'REQUESTED_pt'),
		(0, 3, 'REQUESTED_es')
GO
INSERT INTO [pin_reissue_statuses_language] (pin_reissue_statuses_id, language_id, language_text)
	VALUES(1, 0, 'APPROVED'),
		(1, 1, 'APPROVED_fr'),
		(1, 2, 'APPROVED_pt'),
		(1, 3, 'APPROVED_es')
GO
INSERT INTO [pin_reissue_statuses_language] (pin_reissue_statuses_id, language_id, language_text)
	VALUES(2, 0, 'REJECTED'),
		(2, 1, 'REJECTED_fr'),
		(2, 2, 'REJECTED_pt'),
		(2, 3, 'REJECTED_es')
GO
INSERT INTO [pin_reissue_statuses_language] (pin_reissue_statuses_id, language_id, language_text)
	VALUES(3, 0, 'UPLOADED'),
		(3, 1, 'UPLOADED_fr'),
		(3, 2, 'UPLOADED_pt'),
		(3, 3, 'UPLOADED_es')
GO
INSERT INTO [pin_reissue_statuses_language] (pin_reissue_statuses_id, language_id, language_text)
	VALUES(4, 0, 'EXPIRED'),
		  (4, 1, 'EXPIRED_fr'),
		  (4, 2, 'EXPIRED_pt'),
		  (4, 3, 'EXPIRED_es')
GO

INSERT INTO [audit_action] (audit_action_id, audit_action_name)
	VALUES(10, 'PinReissue')
GO

INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES(10, 0, 'PinReissue')
GO
INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES(10, 1, 'PinReissue_fr')
GO
INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES(10, 2, 'PinReissue_pt')
GO
INSERT INTO [audit_action_language] (audit_action_id, language_id, language_text)
	VALUES(10, 3, 'PinReissue_es')
GO