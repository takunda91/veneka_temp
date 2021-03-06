

MERGE INTO [dbo].[user_roles_language] AS trgt
USING	(VALUES
		(0,0,'CONFIG_ADMIN'),
		(0,1,'CONFIG_ADMIN_fr'),
		(0,2,'CONFIG_ADMIN_pt'),
		(0,3,'CONFIG_ADMIN_es'),
		(1,0,'AUDITOR'),
		(1,1,'AUDITEUR'),
		(1,2,'AUDITOR_pt'),
		(1,3,'AUDITOR_sp'),
		(2,0,'BRANCH_CUSTODIAN'),
		(2,1,'SUPERVISEUR AGENCE'),
		(2,2,'BRANCH_CUSTODIAN_pt'),
		(2,3,'BRANCH_CUSTODIAN_sp'),
		(3,0,'BRANCH_OPERATOR'),
		(3,1,'OPERATEUR AGENCE'),
		(3,2,'BRANCH_OPERATOR_pt'),
		(3,3,'BRANCH_OPERATOR_sp'),
		(4,0,'CENTER_MANAGER'),
		(4,1,'CHEF CARD CENTER'),
		(4,2,'CENTER_MANAGER_pt'),
		(4,3,'CENTER_MANAGER_sp'),
		(5,0,'CENTER_OPERATOR'),
		(5,1,'OPERATEUR CARD CENTER'),
		(5,2,'CENTER_OPERATOR_pt'),
		(5,3,'CENTER_OPERATOR_sp'),
		(6,0,'ISSUER_ADMIN'),
		(6,1,'ADMIN EMETTEUR'),
		(6,2,'ISSUER_ADMIN_pt'),
		(6,3,'ISSUER_ADMIN_sp'),
		(7,0,'PIN_OPERATOR'),
		(7,1,'OPERATEUR PIN'),
		(7,2,'PIN_OPERATOR_pt'),
		(7,3,'PIN_OPERATOR_sp'),
		(8,0,'USER_GROUP_ADMIN'),
		(8,1,'ADMIN GROUPES D''UTILISATEURS'),
		(8,2,'USER_GROUP_ADMIN_pt'),
		(8,3,'USER_GROUP_ADMIN_sp'),
		(9,0,'USER_ADMIN'),
		(9,1,'ADMIN UTILISATEURS'),
		(9,2,'USER_ADMIN_pt'),
		(9,3,'USER_ADMIN_sp'),
		(10,0,'BRANCH_ADMIN'),
		(10,1,'ADMIN AGENCE'),
		(10,2,'BRANCH_ADMIN_pt'),
		(10,3,'BRANCH_ADMIN_sp'),
		(11,0,'CARD_PRODUCTION'),
		(11,1,'CARD_PRODUCTION_fr'),
		(11,2,'CARD_PRODUCTION_pt'),
		(11,3,'CARD_PRODUCTION_es'),
		(12,0,'CMS_OPERATOR'),
		(12,1,'CMS_OPERATOR_fr'),
		(12,2,'CMS_OPERATOR_pt'),
		(12,3,'CMS_OPERATOR_es'),
		(13,0,'PIN_PRINTER_OPERATOR'),
		(13,1,'PIN_PRINTER_OPERATOR_fr'),
		(13,2,'PIN_PRINTER_OPERATOR_pt'),
		(13,3,'PIN_PRINTER_OPERATOR_es'),
		(14,0,'CARD_CENTRE_PIN_OFFICER'),
		(14,1,'CARD_CENTRE_PIN_OFFICER_fr'),
		(14,2,'CARD_CENTRE_PIN_OFFICER_pt'),
		(14,3,'CARD_CENTRE_PIN_OFFICER_es'),
		(15,0,'BRANCH_PIN_OFFICER'),
		(15,1,'BRANCH_PIN_OFFICER_fr'),
		(15,2,'BRANCH_PIN_OFFICER_pt'),
		(15,3,'BRANCH_PIN_OFFICER_es'),
		(16,0,'REPORT_ADMIN'),
		(16,1,'REPORT_ADMIN_fr'),
		(16,2,'REPORT_ADMIN_pt'),
		(16,3,'REPORT_ADMIN_es'),
		(17,0,'USER_AUDIT'),
		(17,1,'USER_AUDIT_fr'),
		(17,2,'USER_AUDIT_pt'),
		(17,3,'USER_AUDIT_es'),
		(18,0,'BRANCH_PRODUCT_OPERATOR'),
		(18,1,'BRANCH_PRODUCT_OPERATOR_fr'),
		(18,2,'BRANCH_PRODUCT_OPERATOR_pt'),
		(18,3,'BRANCH_PRODUCT_OPERATOR_es'),
		(19,0,'BRANCH_PRODUCT_MANAGER'),
		(19,1,'BRANCH_PRODUCT_MANAGER_fr'),
		(19,2,'BRANCH_PRODUCT_MANAGER_pt'),
		(19,3,'BRANCH_PRODUCT_MANAGER_es')
		) AS src([user_role_id],[language_id],[language_text])
ON
	trgt.[user_role_id] = src.[user_role_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[user_role_id] = src.[user_role_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([user_role_id],[language_id],[language_text])
	VALUES ([user_role_id],[language_id],[language_text])

;

