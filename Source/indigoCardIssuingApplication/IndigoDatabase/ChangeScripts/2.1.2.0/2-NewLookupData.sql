INSERT INTO [interface_type] (interface_type_id, interface_type_name)
	VALUES (7, 'NOTIFICATIONS'),
		   (8, 'FILE_EXPORT'),
	       (9, 'REMOTE_CMS')
GO

INSERT INTO [interface_type_language] (interface_type_id, language_id, language_text)
	VALUES(7, 0, 'NOTIFICATIONS'),
		  (7, 1, 'NOTIFICATIONS_fr'),
	      (7, 2, 'NOTIFICATIONS_pt'),
	      (7, 3, 'NOTIFICATIONS_es'),
		  (8, 0, 'FILE_EXPORT'),
		  (8, 1, 'FILE_EXPORT_fr'),
	      (8, 2, 'FILE_EXPORT_pt'),
	      (8, 3, 'FILE_EXPORT_es'),
		  (9, 0, 'REMOTE_CMS'),
		  (9, 1, 'REMOTE_CMS_fr'),
	      (9, 2, 'REMOTE_CMS_pt'),
	      (9, 3, 'REMOTE_CMS_es')
GO

INSERT INTO [response_messages] (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES (0, 37, 'Card order approved.', 'Card order approved._fr', 'Card order approved._pt', 'Card order approved._es')
GO
INSERT INTO [response_messages] (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES (0, 38, 'Card order rejected.', 'Card order rejected._fr', 'Card order rejected._pt', 'Card order rejected._es')
GO

INSERT INTO dist_batch_status_flow (dist_batch_status_flow_id, dist_batch_status_flow_name, dist_batch_type_id, card_issue_method_id)
VALUES (1, 'DEFAULT_CENTRALISED_PRODUCTION', 0, 0),
	   (2, 'DEFAULT_CENTRALISED_PRODUCTION_WITH_PINMAILER', 0, 0),
	   (3, 'DEFAULT_INSTANT_PRODUCTION', 0, 1),
	   (4, 'DEFAULT_INSTANT_EMP_PRODUCTION', 0, 1),
	   (5, 'DEFAULT_CENTRALISED_DISTRIBUTION', 1, 0),
	   (6, 'DEFAULT_INSTANT_DISTRIBUTION', 1, 1)
GO