USE [{DATABASE_NAME}]
GO

/*
GPG PRIVATE AND PUBLIC KEYS FOR ECOBANK CARD FILES
*/
DECLARE @private_key VARCHAR(MAX) = 
'lQOsBFNwj2cBCADRpA+sPLYWNSTWMtXdvrMfi2kP0x/g5WgoRWwK/mlmfrFRN2R6
UsmWZMLKbX/4nBx8nnsAhkkiTzmOftR6p598lRhN5RAXYsh3WuqcxqpDtFxfS3g6
5GaeA49gRDBNG/1oa3MFp9Tx8Vhv84mPtx7X1BiUjymegyKkcwvAt/zFtO4QCqXG
UzY+HvggL8+LcV18FwFwloppQAym6eub4DhOLXZIDlQ+ZjEExsMCuXliWKTKCs98
yvwelJaiFq1+29fn/OAI2bQ7ZvE49YgfXTZ3ObjnXyVk04uuWKUQigFz25jxNpUP
aBb4cGNVZCknro/Er1eVtzuij3z6vKJ6eBFRABEBAAH/AwMCnBSmFG1PgF9gVwzj
a0B9+Xu/O2Uv5XzszwvtRP+zEdppH07IDblqyyaWkyz9cUA7KYDXR3DAeAkVAKnY
UzmAmqmzSgOhoTJktYNqlejw4IGi0H+bn7OQdNXxho9jR2nH/SnhbUITXID+h9bt
DC+UHHHcOrlSkb9QMCj/Hr7aGjRROoE7qUFqWjUXraOD3q3xIJJqSSwzFeGt8LQr
ZG7h1g5aN2CCsPOlAMpuKzKgbzKp0xMDvK2+PLc7PTink+Zj1JAUs449btbbBvqj
rvWVe/qgn5oANLyRJjO40FgVhYj7Ou0wuKe3ATtmJj3Y55TTV5XxeDA6CbNZBfrk
wz7vcrV4/nzQ1OCk4/Q8Wy9i31gRV6Jy4mzwHBy7H0uQvj9eVcahJjAleMNYlfcf
f1S1tGhGNgHRQjf/Xveo5kwW+G7MogZ4Y32ciEWpw4GGdJjxPSQ0BLCXiLM29/KT
rr0w1cAVnepelXt+YCx73vzYRFIHKj/Cwq1HrlVk4GOxmXLiAW6ksbfyGkgbvy/0
fDSFXtfKQERdsdPwwOVy2l2DA7Bvht7vQyq2UsaxcLPjd+mmxzzgjNpECfdWk62X
aLojFE1bc0tfmQhRdxpc8NTw+rroH5MzgO3SmbYz3tfg1Q5wMmqbyatr64zE3Kj4
wNCzD4ATAagon7j1Voevx4pr1ZQwKxWa7ZwIE6DHr/YqINMRmrvqg6uLZ8b7wBVl
ldfOBDNByRHa0fGRlpU20N5Il0W0jV4R70Cqfk/uUnxoEgzhwdLDtZRRMQ3qP2vV
UnwLdCYep2w44kuPupIWavtP9Jd5WHt+NRt+2znb+ZmwYGmuGVZ4wJ3P/N2wqmms
NdNq3Xkcz2eDb96i3zFvEIFuu7z0CHvzTj7rz1SGzbQac2FuZGh5YS5rb25kdXJ1
QHZlbmVrYS5jb22JARwEEAECAAYFAlNwj2cACgkQuIguA3hgnw6cmAf/fe4GG4Mz
xX7Ob94X5QLLn5G0Tn/6GD4je9God0y32N5+G/RKpMSfY1JAUgU9LfduQ6q4V/Te
5gDObd9win6l8xvaueEhs4HXjA6EOMAXYEbUN/S2yL93ovJYaLo8f7GQ45e/y7em
nkZviaHN7uK7CiYZ0MUzlI+EDlpvgFKLTgAotNG9xYKDCKies2Lwk3DL4CMXMde8
yWDglyhRnb9wOHjvnKwdDh7+22b3VnLHrONh8x1GARmvr9lxNOeYYAaXKxxI+2Mr
Ydqcc5MNMX9HD7NSQ1avWmX/JD0qMM43nU2UnyECCEGJMhSpqbEmQ+liFVbvof7o
wLATn5VDMCsfyg==
=tOAg'

DECLARE @public_key VARCHAR(MAX) = 
'mQENBFNwj2cBCADRpA+sPLYWNSTWMtXdvrMfi2kP0x/g5WgoRWwK/mlmfrFRN2R6
UsmWZMLKbX/4nBx8nnsAhkkiTzmOftR6p598lRhN5RAXYsh3WuqcxqpDtFxfS3g6
5GaeA49gRDBNG/1oa3MFp9Tx8Vhv84mPtx7X1BiUjymegyKkcwvAt/zFtO4QCqXG
UzY+HvggL8+LcV18FwFwloppQAym6eub4DhOLXZIDlQ+ZjEExsMCuXliWKTKCs98
yvwelJaiFq1+29fn/OAI2bQ7ZvE49YgfXTZ3ObjnXyVk04uuWKUQigFz25jxNpUP
aBb4cGNVZCknro/Er1eVtzuij3z6vKJ6eBFRABEBAAG0GnNhbmRoeWEua29uZHVy
dUB2ZW5la2EuY29tiQEcBBABAgAGBQJTcI9nAAoJELiILgN4YJ8OnJgH/33uBhuD
M8V+zm/eF+UCy5+RtE5/+hg+I3vRqHdMt9jefhv0SqTEn2NSQFIFPS33bkOquFf0
3uYAzm3fcIp+pfMb2rnhIbOB14wOhDjAF2BG1Df0tsi/d6LyWGi6PH+xkOOXv8u3
pp5Gb4mhze7iuwomGdDFM5SPhA5ab4BSi04AKLTRvcWCgwionrNi8JNwy+AjFzHX
vMlg4JcoUZ2/cDh475ysHQ4e/ttm91Zyx6zjYfMdRgEZr6/ZcTTnmGAGlyscSPtj
K2HanHOTDTF/Rw+zUkNWr1pl/yQ9KjDON51NlJ8hAghBiTIUqamxJkPpYhVW76H+
6MCwE5+VQzArH8o=
=+/pD'

/*
GPG PASS PHRASE
*/
DECLARE @passphrase VARCHAR(30) = 'v3n3ka!'

DECLARE @interface_areas TABLE ([interface_area_id] int not null)
INSERT INTO @interface_areas ([interface_area_id]) VALUES (0),(1)

OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
DECRYPTION BY CERTIFICATE Indigo_Certificate

--Update file connections
UPDATE [dbo].[connection_parameters]
SET  [file_encryption_type_id] = 1
	,[private_key] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),@private_key)
	,[public_key] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),@public_key)
	,[password] = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max), @passphrase))
WHERE [connection_parameter_type_id] = 1

CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

--File Loader linking
INSERT INTO [dbo].[product_interface]
           ([interface_type_id]
           ,[product_id]
           ,[connection_parameter_id]
           ,[interface_area]
           ,[interface_guid])
SELECT 4 as [interface_type_id]
      ,[product_id]
      ,[connection_parameter_id]
      ,0 as [interface_area]
      ,'8DC86A77-BCC7-45C4-8C37-71D7982D6543' as [interface_guid]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer] INNER JOIN [dbo].[connection_parameters]
	ON [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[cards_file_location] = [dbo].[connection_parameters].[path]
		INNER JOIN [dbo].[issuer_product]
	ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id]


--Core Banking linking
INSERT INTO [dbo].[product_interface]
           ([interface_type_id]
           ,[product_id]
           ,[connection_parameter_id]
           ,[interface_area]
           ,[interface_guid])
SELECT 0 as [interface_type_id]
      ,[product_id]
      ,[dbo].[connection_parameters].[connection_parameter_id]
      ,[interface_area].[interface_area_id] as [interface_area]
      ,'16FF4765-409F-4FAE-9D47-6BF7800CF111' as [interface_guid]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer] INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface]
		ON [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[issuer_id]
			AND [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[interface_type_id] = 0
	INNER JOIN [dbo].[connection_parameters]
		ON [dbo].[connection_parameters].[connection_parameter_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[connection_parameter_id]
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id],
	@interface_areas as [interface_area]
	
--CMS linking
INSERT INTO [dbo].[product_interface]
           ([interface_type_id]
           ,[product_id]
           ,[connection_parameter_id]
           ,[interface_area]
           ,[interface_guid])
SELECT 1 as [interface_type_id]
      ,[product_id]
      ,[dbo].[connection_parameters].[connection_parameter_id]
      ,[interface_area].[interface_area_id] as [interface_area]
      ,'4FB6ED60-3A23-47B1-AA95-8EDE28427290' as [interface_guid]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[issuer] INNER JOIN [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface]
		ON [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[issuer_id]
			AND [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[interface_type_id] = 1
	INNER JOIN [dbo].[connection_parameters]
		ON [dbo].[connection_parameters].[connection_parameter_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer_interface].[connection_parameter_id]
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[issuer].[issuer_id],
	@interface_areas as [interface_area]


--User AD linking
UPDATE [new_user]
SET [new_user].[connection_parameter_id] = [dbo].[connection_parameters].[connection_parameter_id],
	[new_user].[workstation] = NULL
FROM [dbo].[user] as [new_user] INNER JOIN [dbo].[connection_parameters]
	ON [new_user].[workstation] = [dbo].[connection_parameters].[connection_name]

--External system and fields
SET IDENTITY_INSERT [dbo].[external_systems] ON 
GO
INSERT [dbo].[external_systems] ([external_system_id], [external_system_type_id], [system_name]) VALUES (1, 2, N'Tieto CMS')
GO
SET IDENTITY_INSERT [dbo].[external_systems] OFF
GO
SET IDENTITY_INSERT [dbo].[external_system_fields] ON
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (1, 1, N'BANK_C')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (2, 1, N'GROUPC')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (3, 1, N'STAT_CHANGE')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (4, 1, N'LIM_INTR')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (5, 1, N'NON_REDUCE_BAL')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (6, 1, N'CRD')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (7, 1, N'CYCLE')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (8, 1, N'DEST_ACCNT_TYPE')
GO
INSERT [dbo].[external_system_fields] ([external_system_field_id], [external_system_id], [field_name]) VALUES (9, 1, N'REP_LANG')
GO
SET IDENTITY_INSERT [dbo].[external_system_fields] OFF
GO

--populate external system fields for products
--BANK_C
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 1, product_id, BANK_C 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--GROUPC
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 2, product_id, GROUPC 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--STAT_CHANGE
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 3, product_id, STAT_CHANGE 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--LIM_INTR
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 4, product_id, LIM_INTR 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--NON_REDUCE_BAL
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 5, product_id, NON_REDUCE_BAL 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--CRD
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 6, product_id, CRD 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--CYCLE
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 7, product_id, CYCLE 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--DEST_ACCNT_TYPE
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 8, product_id, DEST_ACCNT_TYPE 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--REP_LANG
INSERT INTO [dbo].[product_external_system] ([external_system_field_id], [product_id], [field_value])
SELECT 9, product_id, REP_LANG 
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params] 
	INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].[issuer_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_account_params].[issuer_id]
GO

--COND_SET
--1. Do update first
UPDATE [new_product_currency]
SET [new_product_currency].[usr_field_name_1] = 'COND_SET'
	,[new_product_currency].[usr_field_val_1] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_cond_accnt].[COND_SET]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_cond_accnt]
	INNER JOIN [dbo].[currency]
		ON LTRIM(RTRIM([dbo].[currency].[currency_code])) = LTRIM(RTRIM([{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_cond_accnt].[CCY]))
	INNER JOIN [dbo].[product_currency] as [new_product_currency]
		ON [new_product_currency].[product_id] = [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_cond_accnt].[product_id] 
			AND [new_product_currency].[currency_id] = [dbo].[currency].[currency_id]
GO

--2. Insert any missing cond_set to product_currency
INSERT INTO [product_currency] (product_id, currency_id, usr_field_name_1, usr_field_val_1, is_base)
SELECT product_id, currency_id, 'COND_SET', COND_SET, 0
FROM 
	(SELECT product_id, LTRIM(RTRIM(CCY)) AS [CCY], COND_SET
		FROM [{SOURCE_DATABASE_NAME}].[dbo].[mod_interface_cond_accnt]
		EXCEPT
		SELECT [product_id], LTRIM(RTRIM([currency_code])) AS [CCY], usr_field_val_1 AS COND_SET
		FROM [dbo].[product_currency] INNER JOIN [dbo].[currency]
			ON [dbo].[currency].[currency_id] = [dbo].[product_currency].[currency_id]) as [DATA]
		INNER JOIN [dbo].[currency]
		ON [dbo].[currency].[currency_code] = [DATA].[CCY]
GO

--Allow all account type linking
INSERT [dbo].[products_account_types] ([product_id], [account_type_id])
SELECT [product_id], [account_type_id]
FROM [dbo].[issuer_product], [dbo].[customer_account_type]
ORDER BY [product_id], [account_type_id]
GO

--Allow all card issuing reasons
INSERT INTO [dbo].[product_issue_reason] ([product_id], [card_issue_reason_id])
SELECT [product_id], [card_issue_reason_id]
FROM [dbo].[issuer_product], [dbo].[card_issue_reason]
ORDER BY [product_id], [card_issue_reason_id]
GO