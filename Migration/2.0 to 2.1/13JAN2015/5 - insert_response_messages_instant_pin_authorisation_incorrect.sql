USE indigo_database_main_dev
GO

  INSERT INTO [response_messages]
  ([system_response_code]
  ,[system_area]
  ,[english_response]
  ,[french_response]
  ,[portuguese_response]
  ,[spanish_response] )
  VALUES (72
	, 0
	, 'The Authorisation Pin captured is incorrect. Please try again.'
	, 'L''autorisation Pin capturé est incorrect . Se il vous plaît essayer à nouveau.'
	, 'The Authorisation Pin captured is incorrect. Please try again._pt'
	, 'The Authorisation Pin captured is incorrect. Please try again._esp')