

INSERT INTO [dbo].[Issuer_product_font]
           ([font_id]
           ,[font_name]
           ,[resource_path]
           ,[DeletedYN])
	SELECT [font_id]
		  ,[font_name]
		  ,[resource_path]
		  ,[DeletedYN]
	  FROM [indigo_database_group].[dbo].[Issuer_product_font]
	  ORDER BY [font_id] ASC
	  
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

DECLARE @cms_password VARCHAR(MAX),
		@cms_username VARCHAR(MAX),
		@cms_ip VARCHAR(MAX),
		@cms_port INT,
		@cms_path VARCHAR(MAX),
		@flex_ip VARCHAR(MAX),
		@flex_port INT,
		@flex_path VARCHAR(MAX)
		
		
SELECT @cms_username = [indigo_database_group].[dbo].[fn_decrypt_value]([username], DEFAULT),
	   @cms_password = [indigo_database_group].[dbo].[fn_decrypt_value]([password], DEFAULT),
	   @cms_ip = [address],
	   @cms_port = [port],
	   @cms_path = [path]
FROM [indigo_database_group].[dbo].[connection_parameters]
WHERE [connection_name] LIKE '%cms%'

SELECT @flex_ip = [address],
	   @flex_port = [port],
	   @flex_path = [path]
FROM [indigo_database_group].[dbo].[connection_parameters]
WHERE [connection_name] LIKE '%flex%'


EXEC [dbo].[sp_open_keys]
EXEC [indigo_database_group].[dbo].[sp_open_keys]


SET IDENTITY_INSERT [dbo].[connection_parameters] ON 


INSERT [dbo].[connection_parameters] 
(	
	[connection_parameter_id], 
	[connection_name], 
	[address], 
	[port], 
	[path], 
	[protocol], 
	[auth_type], 
	[username], 
	[password], 
	[connection_parameter_type_id], 
	[header_length], 
	[identification], 
	[timeout_milli], 
	[buffer_size], 
	[doc_type], 
	[name_of_file], 
	[file_delete_YN], 
	[file_encryption_type_id], 
	[duplicate_file_check_YN], 
	[private_key], 
	[public_key], 
	[domain_name], 
	[is_external_auth]) 
VALUES 
(
	3, 
	N'ecobank_fileload', 
	N'', 
	0, 
	N'C:\Veneka\indigo_group_ver_2_1_2\card files\', 
	2, 
	0, 
	0x001E655CE1C0704091B8F362C637187A010000009F9116024D42183ED092263E5AD722CE5110EE4AAF2BE4B02A4C7636E5AE59C6, 
	ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max), @passphrase)), 
	1, 
	NULL, 
	0x001E655CE1C0704091B8F362C637187A01000000034D214A06F618C3138A5AAD46215F05D8B5443998E495886315A3CD281825C3, 
	NULL, 
	NULL, 
	NULL, 
	N'', 
	1, 
	1, 
	1, 
	ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max), @private_key)),
	ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max), @public_key)),
	NULL, 
	0
), 
(
	1, 
	N'CMS_conn', 
	@cms_ip, 
	@cms_port, 
	@cms_path, 
	0, 
	0, 
	[dbo].[fn_encrypt_value](@cms_username), 
	[dbo].[fn_encrypt_value](@cms_password), 
	0, 
	NULL, 
	0x001E655CE1C0704091B8F362C637187A01000000B70F011B5F5C96077C4130F86EB42495F0D96AA2044CBC469C69C0DA5D6AF2ED, 
	NULL, 
	NULL, 
	NULL, 
	N'', 
	0, 
	0, 
	0, 
	NULL, 
	NULL, 
	NULL, 
	0
), 
(
	2, 
	N'Flex_conn', 
	@flex_ip, 
	@flex_port, 
	@flex_path, 
	0, 
	0, 
	0x001E655CE1C0704091B8F362C637187A0100000001A8B59D0703CFE56A7A8A29F8BEFC1F393D51735A5A056B844E8EA765C47955, 
	0x001E655CE1C0704091B8F362C637187A01000000E7071D00D9F0FEFFEF7898560A47FB6D2D7E6EBA4BAAF4C9FF9D775448B5BD34, 
	0, 
	NULL, 
	0x001E655CE1C0704091B8F362C637187A01000000C81B1C6AF39746CBC535EAA540397A607769AE42827CFCA21C3AB9A80ED5154A, 
	NULL, 
	NULL, 
	NULL, 
	N'', 
	0, 
	0, 
	0, 
	NULL, 
	NULL, 
	NULL, 
	0
)

SET IDENTITY_INSERT [dbo].[connection_parameters] OFF

EXEC [indigo_database_group].[dbo].[sp_close_keys]
EXEC [dbo].[sp_close_keys]




SET IDENTITY_INSERT  [dbo].[issuer_product] ON 

INSERT INTO [dbo].[issuer_product]
           ([product_id]
		   ,[product_code]
           ,[product_name]
           ,[product_bin_code]
           ,[issuer_id]
           ,[name_on_card_top]
           ,[name_on_card_left]
           ,[Name_on_card_font_size]
           ,[font_id]
           ,[DeletedYN]
           ,[src1_id]
           ,[src2_id]
           ,[src3_id]
           ,[PVKI]
           ,[PVK]
           ,[CVKA]
           ,[CVKB]
           ,[expiry_months]
           ,[fee_scheme_id]
           ,[enable_instant_pin_YN]
           ,[min_pin_length]
           ,[max_pin_length]
           ,[enable_instant_pin_reissue_YN]
           ,[cms_exportable_YN]
           ,[product_load_type_id]
           ,[sub_product_code]
           ,[pin_calc_method_id]
           ,[auto_approve_batch_YN]
           ,[account_validation_YN]
           ,[pan_length]
           ,[pin_mailer_printing_YN]
           ,[pin_mailer_reprint_YN]
           ,[sub_product_id]
           ,[master_product_id]
           ,[card_issue_method_id]
           ,[decimalisation_table]
           ,[pin_validation_data]
           ,[pin_block_formatid])

SELECT [product_id]
	  ,[product_code]
	  ,[product_name]
	  ,SUBSTRING([product_bin_code], 1, 6)
	  ,[issuer_id]
	  ,[name_on_card_top]
	  ,[name_on_card_left]
	  ,[Name_on_card_font_size]
	  ,[font_id]
	  ,[DeletedYN]
	  ,1
	  ,0
	  ,0
      ,NULL
	  ,NULL
	  ,NULL
	  ,NULL
	  ,24
      ,NULL
	  ,0
	  ,4
	  ,4
	  ,0
	  ,0
	  ,2
	  ,SUBSTRING([product_bin_code], 7, LEN([product_bin_code]))
	  ,0
	  ,1
	  ,1
	  ,ISNULL(LEN((SELECT TOP 1 [indigo_database_group].[dbo].[fn_decrypt_value]([card_number], DEFAULT)FROM [dbo].[cards] WHERE [product_id] = [old_product].[product_id])), 16)
	  ,0
	  ,0
	  ,NULL
	  ,NULL
	  ,1
	  ,NULL
	  ,NULL
	  ,NULL
FROM [indigo_database_group].[dbo].[issuer_product] AS [old_product]
ORDER BY [product_id] ASC

SET IDENTITY_INSERT  [dbo].[issuer_product] OFF

INSERT INTO [dbo].[product_currency]
           ([product_id]
           ,[currency_id]
		   ,is_base)
SELECT [product_id]
      ,[currency_id]
	  ,0
FROM [indigo_database_group].[dbo].[product_currency]



/*

SETUP EACH PRODUCT

*/
DECLARE @count INT = 0,
		@currentProdID INT = 0;

DECLARE @productIDs TABLE(id BIGINT)

INSERT INTO @productIDs
	SELECT [product_id]
	FROM [indigo_database_group].[dbo].[issuer_product]
	ORDER BY [product_id]


SET @count = 
(
	SELECT COUNT(*)
	FROM @productIDs
)

WHILE @count > 0
BEGIN
	
	SET @currentProdID = 
	(
		SELECT TOP 1 [id]
		FROM @productIDs
		ORDER BY [id] DESC
	)

	DECLARE @product_name VARCHAR(MAX), 
			@product_id BIGINT
	
	SELECT @product_name = [product_name], @product_id = [product_id]
	FROM [indigo_database_group].[dbo].[issuer_product]
	WHERE [product_id] = @currentProdID
	
			
	INSERT INTO [dbo].[product_interface]
           ([interface_type_id]
           ,[product_id]
           ,[connection_parameter_id]
           ,[interface_area]
           ,[interface_guid])
     VALUES (0, @product_id, 2, 1, '16FF4765-409F-4FAE-9D47-6BF7800CF111'),
			(1, @product_id, 1, 0, '4FB6ED60-3A23-47B1-AA95-8EDE28427290'),
			(1, @product_id, 1, 1, '4FB6ED60-3A23-47B1-AA95-8EDE28427290'),
			(4, @product_id, 3, 0, '8DC86A77-BCC7-45C4-8C37-71D7982D6543')


	INSERT INTO [dbo].[product_issue_reason]
			([product_id], [card_issue_reason_id])
	VALUES (@product_id, 0),
		   (@product_id, 1),
		   (@product_id, 2),
		   (@product_id, 3),
		   (@product_id, 4)


	INSERT INTO [dbo].[products_account_types]
			([product_id], [account_type_id])
	VALUES (@product_id, 0),
		   (@product_id, 1),
		   (@product_id, 2)

	

	DELETE @productIDs
	WHERE [id] = @currentProdID

	SET @count = 
	(
		SELECT COUNT(*)
		FROM @productIDs
	)
END		




-- MOVE THE SEED VALUE FOR SAFTY SAKE
--DECLARE @issuer_product_current_seed INT,
--		@issuer_product_new_seed INT

--SET @issuer_product_current_seed = 
--(
--	SELECT TOP 1 [issuer_product].[product_id]
--	FROM [dbo].[issuer_product]
--	ORDER BY [issuer_product].[product_id] DESC
--)

--SET @issuer_product_new_seed = (@issuer_product_current_seed * 1.5)


--DBCC CHECKIDENT('[dbo].[issuer_product]', RESEED, @issuer_product_new_seed);
