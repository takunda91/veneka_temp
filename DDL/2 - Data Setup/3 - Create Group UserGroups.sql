USE [indigo_database_group]
GO

INSERT INTO [dbo].[user_group]
           ([user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name])
     VALUES (0, -1, 1, 1, 1, 1, 1, 'GROUP_ADMINISTRATOR')
GO

INSERT INTO [dbo].[user_group]
           ([user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name])
     VALUES (1, -1, 1, 1, 1, 1, 1, 'GROUP_AUDITOR')
GO

INSERT INTO [dbo].[user_group]
           ([user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name])
     VALUES (6, -1, 1, 1, 1, 1, 1, 'GROUP_ISSUER_ADMIN')
GO

INSERT INTO [dbo].[user_group]
           ([user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name])
     VALUES (10, -1, 1, 1, 1, 1, 1, 'GROUP_BRANCH_ADMIN')
GO
INSERT INTO [dbo].[user_group]
           ([user_role_id],[issuer_id],[can_create],[can_read],[can_update],[can_delete],[all_branch_access],[user_group_name])
     VALUES (11, -1, 1, 1, 1, 1, 1, 'GROUP_CARD_PRODUCTION')
GO



