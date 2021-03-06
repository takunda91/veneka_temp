

MERGE INTO [dbo].[branch_card_codes_language] AS trgt
USING	(VALUES
		(0,0,'PRINT_SUCCESS'),
		(1,0,'PRINTER_JAMMED'),
		(2,0,'CARD_INSERTED_INCORRECTLY'),
		(3,0,'PRINTER_NO_INK'),
		(4,0,'CMS_SUCCESS'),
		(5,0,'CARD_NOT_FOUND'),
		(6,0,'ACCOUNT_NOT_FOUND'),
		(7,0,'UNKNOWN_ERROR'),
		(8,0,'RELINK_FAILED'),
		(9,0,'EDIT_FAILED'),
		(0,1,'Impression a été faite avec succès'),
		(1,1,'IMPRIMANTE EN BOURRAGE'),
		(2,1,'CARTE MAL INSEREE'),
		(3,1,'ANKRE INSUFFISANT'),
		(4,1,'OPERATIONS'),
		(5,1,'CARTE INTROUVABLE'),
		(6,1,'COMPTE INTROUVABLE'),
		(7,1,'ERREUR ETRANGE'),
		(8,1,'RELIAGE A ECHOUE'),
		(9,1,'MODIFICATION A ECHOUEE'),
		(0,2,'PRINTER_JAMMED_fr'),
		(1,2,'PRINTER_JAMMED_pt'),
		(2,2,'CARD_INSERTED_INCORRECTLY_pt'),
		(3,2,'PRINTER_NO_INK_pt'),
		(4,2,'CMS_SUCCESS_pt'),
		(5,2,'CARD_NOT_FOUND_pt'),
		(6,2,'ACCOUNT_NOT_FOUND_pt'),
		(7,2,'UNKNOWN_ERROR_pt'),
		(8,2,'RELINK_FAILED_pt'),
		(9,2,'EDIT_FAILED_pt'),
		(0,3,'PRINT_SUCCESS_sp'),
		(2,3,'CARD_INSERTED_INCORRECTLY_sp'),
		(3,3,'PRINTER_NO_INK_sp'),
		(4,3,'CMS_SUCCESS_sp'),
		(5,3,'CARD_NOT_FOUND_sp'),
		(6,3,'ACCOUNT_NOT_FOUND_sp'),
		(7,3,'UNKNOWN_ERROR_sp'),
		(8,3,'RELINK_FAILED_sp'),
		(9,3,'EDIT_FAILED_sp')
		) AS src([branch_card_code_id],[language_id],[language_text])
ON
	trgt.[branch_card_code_id] = src.[branch_card_code_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[branch_card_code_id] = src.[branch_card_code_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([branch_card_code_id],[language_id],[language_text])
	VALUES ([branch_card_code_id],[language_id],[language_text])

;

