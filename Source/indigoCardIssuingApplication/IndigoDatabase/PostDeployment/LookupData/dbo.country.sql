SET IDENTITY_INSERT [dbo].[country] ON;

MERGE INTO [dbo].[country] AS trgt
USING	(VALUES
		(1,'COTE D''IVOIRE','CIV','Yamoussoukro'),
		(2,'GHANA','GHA','ACCRA'),
		(3,'GUINEA-BISSAU','GNB','Bissau'),
		(4,'TOGO','TGO','Lome'),
		(5,'BENIN','BEN','Porto-Novo'),
		(9,'MALI','MAL','Bamako'),
		(10,'KENYA','KEN','Nairobi'),
		(11,'BURKINA FASO','BFA','Ouagadougou'),
		(12,'TANZANIA','TZA','Dar Es Salaam'),
		(36,'BURUNDI','BDI','Bujumbura'),
		(37,'CAMEROUN','CMR','Yaoundé'),
		(38,'CAPE VERDE','CPV','Praia'),
		(39,'CENTRAL AFRICAN REPUBLIC','CAF','Bangui'),
		(40,'CHAD','TCD','N’Djamena'),
		(41,'CONGO BRAZZA','COG','Brazzaville'),
		(42,'Democratic Republic of Congo','COD','Kinshasa'),
		(43,'EQUATORIAL GUINEA','GNQ','Malabo'),
		(44,'GABON','GAB','Libreville'),
		(45,'GAMBIA','GMB','Banjul'),
		(46,'GUINEA CONAKRY','GIN','Conakry'),
		(47,'LIBERIA','LBR','Monrovia'),
		(48,'MALAWI','MWI','Lilongwe'),
		(49,'MOZAMBIQUE','MOZ','Maputo'),
		(50,'NIGER','NER','Niamey'),
		(51,'RWANDA','RWA','Kigali'),
		(52,'SAO TOME','STP','Sao Tome'),
		(53,'SIERRA LEONE','SLE','Freetown'),
		(54,'SENEGAL','SEN','Dakar'),
		(55,'SOUTH SUDAN','SSD','Juba'),
		(56,'UGANDA','UGA','Kampala'),
		(57,'ZAMBIA','ZMB','Lusaka'),
		(58,'ZIMBABWE','ZWE','Harare')
		) AS src([country_id],[country_name],[country_code],[country_capital_city])
ON
	trgt.[country_id] = src.[country_id]
WHEN MATCHED THEN
	UPDATE SET
		[country_name] = src.[country_name]
		, [country_code] = src.[country_code]
		, [country_capital_city] = src.[country_capital_city]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([country_id],[country_name],[country_code],[country_capital_city])
	VALUES ([country_id],[country_name],[country_code],[country_capital_city])

;
SET IDENTITY_INSERT [dbo].[country] OFF;
