CREATE VIEW [dbo].[user_roles_branch_pending]
AS
SELECT        branch_id, [pending_user_id] as [user_id] , user_role_id, [user_group].issuer_id
FROM            [user_groups_branches] INNER JOIN
                         [user_group] ON [user_groups_branches].user_group_id = [user_group].user_group_id AND [user_group].all_branch_access = 0 INNER JOIN
                         [user_to_user_group_pending] ON [user_group].user_group_id = [user_to_user_group_pending].user_group_id
UNION
SELECT        branch_id, [pending_user_id] as[user_id], user_role_id, [branch].issuer_id
FROM            [branch] INNER JOIN
                         [user_group] ON [user_group].issuer_id = [branch].issuer_id AND [user_group].all_branch_access = 1 INNER JOIN
                         [user_to_user_group_pending] ON [user_group].user_group_id = [user_to_user_group_pending].user_group_id
UNION
SELECT        branch_id,[pending_user_id]as [user_id], user_role_id, [branch].issuer_id
FROM            [branch], [user_group] INNER JOIN
                         [user_to_user_group_pending] ON [user_group].user_group_id = [user_to_user_group_pending].user_group_id AND EXISTS
                             (SELECT        sug.*
                               FROM            [user_group] sug
                               WHERE        sug.issuer_id = - 1 AND sug.user_group_id = [user_group].user_group_id)