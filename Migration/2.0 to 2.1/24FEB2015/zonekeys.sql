USE [indigo_database_main_dev]
GO

-- This is the certificate that will protect our ZML keys
CREATE CERTIFICATE cert_ZoneMasterKeys WITH SUBJECT = 'ZMK Protection'
GO 
-- This key will be used to protect the MAC keys
CREATE SYMMETRIC KEY key_injection_keys WITH ALGORITHM = AES_256
ENCRYPTION BY CERTIFICATE cert_ZoneMasterKeys
GO

CREATE TABLE [dbo].[zone_keys](
	[issuer_id] [int] NOT NULL,
	[zone] [varbinary](max) NOT NULL,
	[final] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_zone_keys] PRIMARY KEY CLUSTERED 
(
	[issuer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO