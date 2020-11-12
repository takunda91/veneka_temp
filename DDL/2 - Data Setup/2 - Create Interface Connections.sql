USE [indigo_database_group]
GO

OPEN SYMMETRIC KEY Indigo_Symmetric_Key
DECRYPTION BY CERTIFICATE Indigo_Certificate

	INSERT INTO [dbo].[connection_parameters]
			   ([connection_name],[address],[port],[path],[protocol],[auth_type],[username],[password])
		 VALUES ('Flexcube_conn', '10.0.0.1', 5757, '/EcoFlex', 1, 0, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'')), ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'')))
	GO

	INSERT INTO [dbo].[connection_parameters]
			   ([connection_name],[address],[port],[path],[protocol],[auth_type],[username],[password])
		 VALUES ('CMS_conn', '10.0.0.1', 8443, '/cs_cms_ws/services/Issuing', 1, 1, 
				  ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'')),
				  ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,'')))
	GO

CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;


