USE [{DATABASE_NAME}]
GO

--Update sequence
INSERT INTO [dbo].[sequences] (sequence_name, last_sequence_number, last_updated)
SELECT 'FlexcubeRequestId', [request_id], GETDATE()
FROM [{SOURCE_DATABASE_NAME}].[dbo].[flex_parameters]
GO

--Load data

SET IDENTITY_INSERT [dbo].[mod_flex_parameters] ON; 

INSERT INTO [dbo].[mod_flex_parameters] (flex_parameter_id, request_token, request_type, source_channel_id, source_code)
SELECT 1, request_token, request_type, source_channel_id, source_code
FROM [{SOURCE_DATABASE_NAME}].[dbo].[flex_parameters]

SET IDENTITY_INSERT [dbo].[mod_flex_parameters] OFF; 
GO

SET IDENTITY_INSERT [dbo].[mod_flex_responses] ON; 

INSERT INTO [dbo].[mod_flex_responses] (flex_response_id, flex_response_name)
SELECT [flex_response_id], [flex_response_name]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[flex_responses]

SET IDENTITY_INSERT [dbo].[mod_flex_responses] OFF;
GO

INSERT INTO [dbo].[mod_flex_response_values] (flex_response_id, flex_response_value_id, flex_response_value, valid_response)
SELECT flex_response_id, flex_response_value_id, flex_response_value, valid_response
FROM [{SOURCE_DATABASE_NAME}].[dbo].[flex_response_values]
GO

INSERT INTO [dbo].[mod_flex_response_values_language] (flex_response_value_id, language_id, language_text)
SELECT flex_response_value_id, language_id, language_text
FROM [{SOURCE_DATABASE_NAME}].[dbo].[flex_response_values_language]
GO