[{DATABASE_NAME}].[dbo].[user_groups_branches]
SELECT [user_group_id], [branch_id]
FROM [{SOURCE_DATABASE_NAME}].[dbo].[user_groups_branches]
ORDER BY [user_group_id] ASC