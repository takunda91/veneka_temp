SET IDENTITY_INSERT [dbo].[user_group] ON;

MERGE INTO [dbo].[user_group] AS trgt
USING	(VALUES
		(-4,17,-1,1,1,1,1,1,'GROUP_USER_AUDIT',1,1),
		(-3,-1,-1,0,0,0,0,1,'INDIGO SYSTEM',1,1),
		(-2,9,-1,1,1,1,1,1,'GROUP_USER_ADMIN',1,1),
		(-1,8,-1,1,1,1,1,1,'GROUP_USER_GROUP_ADMIN',1,1)
		) AS src([user_group_id],[user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name],[mask_screen_pan],[mask_report_pan])
ON
	trgt.[user_group_id] = src.[user_group_id]
WHEN MATCHED THEN
	UPDATE SET
		[user_role_id] = src.[user_role_id]
		, [issuer_id] = src.[issuer_id]
		, [can_create] = src.[can_create]
		, [can_read] = src.[can_read]
		, [can_update] = src.[can_update]
		, [can_delete] = src.[can_delete]
		, [all_branch_access] = src.[all_branch_access]
		, [user_group_name] = src.[user_group_name]
		, [mask_screen_pan] = src.[mask_screen_pan]
		, [mask_report_pan] = src.[mask_report_pan]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([user_group_id],[user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name],[mask_screen_pan],[mask_report_pan])
	VALUES ([user_group_id],[user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name],[mask_screen_pan],[mask_report_pan])

;
SET IDENTITY_INSERT [dbo].[user_group] OFF;
