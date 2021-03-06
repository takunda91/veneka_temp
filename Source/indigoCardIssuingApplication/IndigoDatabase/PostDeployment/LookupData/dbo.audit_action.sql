

MERGE INTO [dbo].[audit_action] AS trgt
USING	(VALUES
		(0,'BranchAdmin'),
		(1,'CardManagement'),
		(2,'DistributionBatch'),
		(3,'IssueCard'),
		(4,'IssuerAdmin'),
		(5,'LoadBatch'),
		(6,'Logon'),
		(7,'UserAdmin'),
		(8,'UserGroupAdmin'),
		(9,'Administration'),
		(10,'PinReissue'),
		(11,'ExportBatch'),
		(12,'3DSecureBatch')
		) AS src([audit_action_id],[audit_action_name])
ON
	trgt.[audit_action_id] = src.[audit_action_id]
WHEN MATCHED THEN
	UPDATE SET
		[audit_action_id] = src.[audit_action_id]
		, [audit_action_name] = src.[audit_action_name]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([audit_action_id],[audit_action_name])
	VALUES ([audit_action_id],[audit_action_name])

;

