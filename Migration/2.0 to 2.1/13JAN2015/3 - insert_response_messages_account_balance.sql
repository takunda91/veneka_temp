USE indigo_database_main_dev
GO

  INSERT INTO [response_messages]
  ([system_response_code]
  ,[system_area]
  ,[english_response]
  ,[french_response]
  ,[portuguese_response]
  ,[spanish_response] )
  VALUES (507
	, 0
	, 'The customer has insuffiecient funds for card fees.'
	, 'Le client dispose de fonds insuffiecient pour les frais de carte.'
	, 'The customer has insuffiecient funds for card fees.'
	, 'The customer has insuffiecient funds for card fees.')