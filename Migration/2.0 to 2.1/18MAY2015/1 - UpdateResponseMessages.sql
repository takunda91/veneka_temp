USE [indigo_database_main_dev]
GO

INSERT INTO [response_messages] (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES (702, 0, 'Cannot delete fee scheme, products still linked to fee scheme.',
				'Cannot delete fee scheme, products still linked to fee scheme._fr',
				'Cannot delete fee scheme, products still linked to fee scheme._pt',
				'Cannot delete fee scheme, products still linked to fee scheme._es')

INSERT INTO [response_messages] (system_response_code, system_area, english_response, french_response, portuguese_response, spanish_response)
VALUES (703, 0, 'Cannot delete fee scheme, sub-products still linked to fee scheme.',
				'Cannot delete fee scheme, sub-products still linked to fee scheme._fr',
				'Cannot delete fee scheme, sub-products still linked to fee scheme._pt',
				'Cannot delete fee scheme, sub-products still linked to fee scheme._es')