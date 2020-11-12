USE [indigo_database_main_dev]
GO

ALTER TABLE [user_groups_branches]
	ADD PRIMARY KEY (user_group_id, branch_id)
GO

ALTER TABLE [user_password_history]
	ADD PRIMARY KEY ([user_id], [date_changed])
GO

ALTER TABLE [users_to_users_groups]
	ADD PRIMARY KEY ([user_id], [user_group_id])
GO