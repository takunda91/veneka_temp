USE [indigo_database_main_dev]
GO

INSERT INTO [response_messages] (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES ('800', '0', 'PIN Reissue not in correct status.', 
					'PIN Reissue not in correct status._fr', 
					'PIN Reissue not in correct status._pt', 
					'PIN Reissue not in correct status._es')
GO

INSERT INTO [response_messages] (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES ('801', '0', 'PIN Reissue request expired, please restart pin request.', 
					'PIN Reissue request expired, please restart pin request._fr', 
					'PIN Reissue request expired, please restart pin request._pt', 
					'PIN Reissue request expired, please restart pin request._es')
GO

INSERT INTO [response_messages] (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES ('802', '0', 'PIN Reissue already requested for card, please complete request.', 
					'PIN Reissue already requested for card, please complete request._fr', 
					'PIN Reissue already requested for card, please complete request._pt', 
					'PIN Reissue already requested for card, please complete request._es')