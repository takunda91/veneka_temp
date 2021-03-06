

MERGE INTO [dbo].[audit_action_language] AS trgt
USING	(VALUES
		(0,0,'BranchAdmin'),
		(0,1,'Admin_Agence'),
		(0,2,'BranchAdmin_pt'),
		(0,3,'BranchAdmin_sp'),
		(1,0,'CardManagement'),
		(1,1,'Gestion des cartes'),
		(1,2,'CardManagement_pt'),
		(1,3,'CardManagement_sp'),
		(2,0,'DistributionBatch'),
		(2,1,'Batch des distributions'),
		(2,2,'DistributionBatch_pt'),
		(2,3,'DistributionBatch_sp'),
		(3,0,'IssueCard'),
		(3,1,'Emettre carte'),
		(3,2,'IssueCard_pt'),
		(3,3,'IssueCard_sp'),
		(4,0,'IssuerAdmin'),
		(4,1,'Admin_Emetteur'),
		(4,2,'IssuerAdmin_pt'),
		(4,3,'IssuerAdmin_sp'),
		(5,0,'LoadBatch'),
		(5,1,'Batch chargé'),
		(5,2,'LoadBatch_pt'),
		(5,3,'LoadBatch_sp'),
		(6,0,'Logon'),
		(6,1,'Se connecter'),
		(6,2,'Logon_pt'),
		(6,3,'Logon_sp'),
		(7,0,'UserAdmin'),
		(7,1,'Utilisateur Admin'),
		(7,2,'UserAdmin_pt'),
		(7,3,'UserAdmin_sp'),
		(8,0,'UserGroupAdmin'),
		(8,1,'Utilisateur Admin Groupe'),
		(8,2,'UserGroupAdmin_pt'),
		(8,3,'UserGroupAdmin_sp'),
		(9,0,'Administration'),
		(9,1,'Administration_fr'),
		(9,2,'Administration_pt'),
		(9,3,'Administration_es'),
		(10,0,'PinReissue'),
		(10,1,'PinReissue_fr'),
		(10,2,'PinReissue_pt'),
		(10,3,'PinReissue_es'),
		(11,0,'ExportBatch'),
		(11,1,'ExportBatch_fr'),
		(11,2,'ExportBatch_pt'),
		(11,3,'ExportBatch_es'),
		(12,0,'3DSecureBatch'),
		(12,1,'3DSecureBatch_fr'),
		(12,2,'3DSecureBatch_pt'),
		(12,3,'3DSecureBatch_es')
		) AS src([audit_action_id],[language_id],[language_text])
ON
	trgt.[audit_action_id] = src.[audit_action_id]
AND trgt.[language_id] = src.[language_id]
WHEN MATCHED THEN
	UPDATE SET
		[audit_action_id] = src.[audit_action_id]
		, [language_id] = src.[language_id]
		, [language_text] = src.[language_text]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([audit_action_id],[language_id],[language_text])
	VALUES ([audit_action_id],[language_id],[language_text])

;

