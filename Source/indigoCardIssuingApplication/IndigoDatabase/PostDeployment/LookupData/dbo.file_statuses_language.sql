

MERGE INTO [dbo].[file_statuses_language] AS trgt
USING	(VALUES
		(0,0,'READ'),
		(0,1,'LIRE'),
		(0,2,'READ_pt'),
		(0,3,'READ_sp'),
		(1,0,'VALID_CARDS'),
		(1,1,'CARTES VALIDES'),
		(1,2,'VALID_CARDS_pt'),
		(1,3,'VALID_CARDS_sp'),
		(2,0,'PROCESSED'),
		(2,1,'TRAITE'),
		(2,2,'PROCESSED_pt'),
		(2,3,'PROCESSED_sp'),
		(3,0,'DUPLICATE_FILE'),
		(3,1,'DOSSIER DUPLIQUE'),
		(3,2,'DUPLICATE_FILE_pt'),
		(3,3,'DUPLICATE_FILE_sp'),
		(4,0,'LOAD_FAIL'),
		(4,1,'LE CHARGEMENT A ECHOUE'),
		(4,2,'LOAD_FAIL_pt'),
		(4,3,'LOAD_FAIL_sp'),
		(5,0,'FILE_CORRUPT'),
		(5,1,'FILE CORROMPUE'),
		(5,2,'FILE_CORRUPT_pt'),
		(5,3,'FILE_CORRUPT_sp'),
		(6,0,'PARTIAL_LOAD'),
		(6,1,'CHARGEMENT PARTIEL'),
		(6,2,'PARTIAL_LOAD_pt'),
		(6,3,'PARTIAL_LOAD_sp'),
		(7,0,'INVALID_FORMAT'),
		(7,1,'FORMAT INVALIDE'),
		(7,2,'INVALID_FORMAT_pt'),
		(7,3,'INVALID_FORMAT_sp'),
		(8,0,'INVALID_NAME'),
		(8,1,'NOM INVALIDE'),
		(8,2,'INVALID_NAME_pt'),
		(8,3,'INVALID_NAME_sp'),
		(9,0,'ISSUER_NOT_FOUND'),
		(9,1,'Emetteur INTROUVABLE'),
		(9,2,'ISSUER_NOT_FOUND_pt'),
		(9,3,'ISSUER_NOT_FOUND_sp'),
		(10,0,'INVALID_ISSUER_LICENSE'),
		(10,1,'LICENSE DE Emetteur INVALIDE'),
		(10,2,'INVALID_ISSUER_LICENSE_pt'),
		(10,3,'INVALID_ISSUER_LICENSE_sp'),
		(11,0,'NO_ACTIVE_BRANCH_FOUND'),
		(11,1,'AUCUNE AGENCE ACTIVE TROUVEE'),
		(11,2,'NO_ACTIVE_BRANCH_FOUND_pt'),
		(11,3,'NO_ACTIVE_BRANCH_FOUND_sp'),
		(12,0,'DUPLICATE_CARDS_IN_FILE'),
		(12,1,'CARTES DUPLIQUEES DANS LE FICHIER'),
		(12,2,'DUPLICATE_CARDS_IN_FILE_pt'),
		(12,3,'DUPLICATE_CARDS_IN_FILE_sp'),
		(13,0,'DUPLICATE_CARDS_IN_DATABASE'),
		(13,1,'CARTES DUPLIQUEES DANS LA BASE DE DONNEES'),
		(13,2,'DUPLICATE_CARDS_IN_DATABASE_pt'),
		(13,3,'DUPLICATE_CARDS_IN_DATABASE_sp'),
		(14,0,'FILE_DECRYPTION_FAILED'),
		(14,1,'DECRYPTAGE DE FICHIER A ECHOUE'),
		(14,2,'FILE_DECRYPTION_FAILED_pt'),
		(14,3,'FILE_DECRYPTION_FAILED_sp'),
		(15,0,'UNLICENSED_BIN'),
		(15,1,'BIN NON AUTORISE'),
		(15,2,'UNLICENSED_BIN_pt'),
		(15,3,'UNLICENSED_BIN_sp'),
		(16,0,'NO_PRODUCT_FOUND_FOR_CARD'),
		(16,1,'AUCUN PRODUIT TROUVE POUR LA CARTE'),
		(16,2,'NO_PRODUCT_FOUND_FOR_CARD_pt'),
		(16,3,'NO_PRODUCT_FOUND_FOR_CARD_sp'),
		(17,0,'UNLICENSED_ISSUER'),
		(17,1,'EMETTEUR NON AUTORISE'),
		(17,2,'UNLICENSED_ISSUER_pt'),
		(17,3,'UNLICENSED_ISSUER_sp'),
		(18,0,'BRANCH_PRODUCT_NOT_FOUND'),
		(18,1,'PRODUIT D''AGENCE INTROUVABLE'),
		(18,2,'BRANCH_PRODUCT_NOT_FOUND_pt'),
		(18,3,'BRANCH_PRODUCT_NOT_FOUND_sp'),
		(19,0,'CARD_FILE_INFO_READ_ERROR'),
		(19,1,'ERREUR DE  LECTURE DES INFORMATION DU FICHIER DES CARTES'),
		(19,2,'CARD_FILE_INFO_READ_ERROR_pt'),
		(19,3,'CARD_FILE_INFO_READ_ERROR_sp'),
		(20,0,'CARDS_NOT_ORDERED'),
		(20,1,'CARDS_NOT_ORDERED_fr'),
		(20,2,'CARDS_NOT_ORDERED_pt'),
		(20,3,'CARDS_NOT_ORDERED_es'),
		(21,0,'ORDERED_CARD_REF_MISSING'),
		(21,1,'ORDERED_CARD_REF_MISSING_fr'),
		(21,2,'ORDERED_CARD_REF_MISSING_pt'),
		(21,3,'ORDERED_CARD_REF_MISSING_es'),
		(22,0,'ORDERED_CARD_PRODUCT_MISS_MATCH'),
		(22,1,'ORDERED_CARD_PRODUCT_MISS_MATCH_fr'),
		(22,2,'ORDERED_CARD_PRODUCT_MISS_MATCH_pt'),
		(22,3,'ORDERED_CARD_PRODUCT_MISS_MATCH_es'),
		(23,0,'MULTIPLE_PRODUCTS_IN_FILE'),
		(23,1,'MULTIPLE_PRODUCTS_IN_FILE_fr'),
		(23,2,'MULTIPLE_PRODUCTS_IN_FILE_pt'),
		(23,3,'MULTIPLE_PRODUCTS_IN_FILE_es'),
		(24,0,'PRODUCT_NOT_ACTIVE'),
		(24,1,'PRODUCT_NOT_ACTIVE_fr'),
		(24,2,'PRODUCT_NOT_ACTIVE_pt'),
		(24,3,'PRODUCT_NOT_ACTIVE_es'),
		(25,0,'NO_LOAD_FOR_PRODUCT'),
		(25,1,'NO_LOAD_FOR_PRODUCT_fr'),
		(25,2,'NO_LOAD_FOR_PRODUCT_pt'),
		(25,3,'NO_LOAD_FOR_PRODUCT_es'),
		(26,0,'ISSUER_NOT_ACTIVE'),
		(26,1,'ISSUER_NOT_ACTIVE_fr'),
		(26,2,'ISSUER_NOT_ACTIVE_pt'),
		(26,3,'ISSUER_NOT_ACTIVE_es'),
		(27,0,'ISSUER_LICENCE_EXPIRED'),
		(27,1,'ISSUER_LICENCE_EXPIRED_fr'),
		(27,2,'ISSUER_LICENCE_EXPIRED_pt'),
		(27,3,'ISSUER_LICENCE_EXPIRED_es')
		) AS src([file_status_id],[language_id],[language_text])
ON
	trgt.[file_status_id] = src.[file_status_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[file_status_id] = src.[file_status_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([file_status_id],[language_id],[language_text])
	VALUES ([file_status_id],[language_id],[language_text])

;

