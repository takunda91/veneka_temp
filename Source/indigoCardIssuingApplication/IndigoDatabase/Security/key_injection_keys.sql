﻿CREATE SYMMETRIC KEY [key_injection_keys]
    AUTHORIZATION [dbo]
    WITH ALGORITHM = AES_256
    ENCRYPTION BY CERTIFICATE [cert_ZoneMasterKeys];
