[{DATABASE_NAME}].[dbo].[issuer_interface]
	SELECT [interface_type_id]
		  ,[issuer_id]
		  ,[connection_parameter_id]
		  ,[interface_area]
		  ,[interface_guid]
	FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface]