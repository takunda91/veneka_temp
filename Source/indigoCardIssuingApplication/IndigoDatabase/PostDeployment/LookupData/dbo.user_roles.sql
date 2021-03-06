

MERGE INTO [dbo].[user_roles] AS trgt
USING	(VALUES
		(-1,'INTERNAL_SYSTEM',0,1),
		(0,'CONFIG_ADMIN',0,0),
		(1,'AUDITOR',0,0),
		(2,'BRANCH_CUSTODIAN',0,0),
		(3,'BRANCH_OPERATOR',0,0),
		(4,'CENTER_MANAGER',0,0),
		(5,'CENTER_OPERATOR',0,0),
		(6,'ISSUER_ADMIN',0,0),
		(7,'PIN_OPERATOR',0,0),
		(8,'USER_GROUP_ADMIN',0,1),
		(9,'USER_ADMIN',1,1),
		(10,'BRANCH_ADMIN',0,0),
		(11,'CARD_PRODUCTION',0,0),
		(12,'CMS_OPERATOR',0,0),
		(13,'PIN_PRINTER_OPERATOR',0,0),
		(14,'CARD_CENTRE_PIN_OFFICER',0,0),
		(15,'BRANCH_PIN_OFFICER',0,0),
		(16,'REPORT_ADMIN',0,0),
		(17,'USER_AUDIT',0,0),
		(18,'BRANCH_PRODUCT_OPERATOR',0,0),
		(19,'BRANCH_PRODUCT_MANAGER',0,0)
		) AS src([user_role_id],[user_role],[allow_multiple_login],[enterprise_only])
ON
	trgt.[user_role_id] = src.[user_role_id]
WHEN MATCHED THEN
	UPDATE SET
		[user_role_id] = src.[user_role_id]
		, [user_role] = src.[user_role]
		, [allow_multiple_login] = src.[allow_multiple_login]
		, [enterprise_only] = src.[enterprise_only]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([user_role_id],[user_role],[allow_multiple_login],[enterprise_only])
	VALUES ([user_role_id],[user_role],[allow_multiple_login],[enterprise_only])

;

