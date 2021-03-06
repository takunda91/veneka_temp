USE [{DATABASE_NAME}]
GO
--Create Master Key
CREATE MASTER KEY ENCRYPTION BY PASSWORD = '{MASTER_KEY}'
GO

--Create General encryption key
CREATE CERTIFICATE Indigo_Certificate WITH SUBJECT = 'Data protection', EXPIRY_DATE = '{EXPITY_DATE}';  
GO

CREATE SYMMETRIC KEY Indigo_Symmetric_Key WITH ALGORITHM = AES_256 
ENCRYPTION BY CERTIFICATE Indigo_Certificate
GO

--Create Zone encryption key
CREATE CERTIFICATE cert_ZoneMasterKeys WITH SUBJECT = 'Zone protection', EXPIRY_DATE = '{EXPITY_DATE}';  
GO

CREATE SYMMETRIC KEY key_injection_keys WITH ALGORITHM = AES_256 
ENCRYPTION BY CERTIFICATE cert_ZoneMasterKeys
GO

-- This is the certificate that will protect our MAC key-encryption key
CREATE CERTIFICATE cert_ProtectIndexingKeys WITH SUBJECT = 'Data indexing key protection', EXPIRY_DATE = '{EXPITY_DATE}'; 
GO 
-- This key will be used to protect the MAC keys
CREATE SYMMETRIC KEY key_Indexing WITH ALGORITHM = AES_256
ENCRYPTION BY CERTIFICATE cert_ProtectIndexingKeys
GO