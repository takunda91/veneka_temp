USE [indigo_database_main_dev]
GO

UPDATE [response_messages]
SET english_response = 'Cannot delete master key. Key in use on terminals.',
	french_response = 'Cannot delete master key. Key is used on terminal_fr.',
	portuguese_response = 'Cannot delete master key. Key is used on terminal._pt',
	spanish_response = 'Cannot delete master key. Key is used on terminal._sp'
where system_response_code = 608


