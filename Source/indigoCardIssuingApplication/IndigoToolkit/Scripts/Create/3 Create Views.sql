USE [{DATABASE_NAME}]
GO
/****** Object:  View [dbo].[avail_cc_and_load_cards]    Script Date: 2018-05-08 08:01:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[avail_cc_and_load_cards]
AS
SELECT card_id
FROM [dist_batch_cards]
WHERE [dist_batch_cards].dist_card_status_id = 18
UNION 
SELECT card_id
FROM [load_batch_cards]	
WHERE [load_batch_cards].load_card_status_id = 1

GO
/****** Object:  View [dbo].[branch_card_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[branch_card_status_current]
AS
SELECT        --dbo.branch_card_status.branch_card_status_id, 
					dbo.branch_card_status.card_id, dbo.cards.card_priority_id,
					dbo.cards.product_id, dbo.cards.card_issue_method_id, dbo.cards.branch_id,
						 dbo.branch_card_status.branch_card_statuses_id, 
                         dbo.branch_card_status.status_date, dbo.branch_card_status.user_id, dbo.branch_card_status.operator_user_id, dbo.branch_card_status.branch_card_code_id, 
                         dbo.branch_card_status.comments, dbo.branch_card_codes.branch_card_code_id AS Expr1, dbo.branch_card_codes.branch_card_code_type_id, 
                         dbo.branch_card_codes.branch_card_code_name, dbo.branch_card_codes.branch_card_code_enabled, dbo.branch_card_codes.spoil_only, 
                         dbo.branch_card_codes.is_exception
FROM         dbo.cards 
				INNER JOIN
				   dbo.branch_card_status ON dbo.cards.card_id = dbo.branch_card_status.card_id
			   LEFT OUTER JOIN
                         dbo.branch_card_codes ON dbo.branch_card_status.branch_card_code_id = dbo.branch_card_codes.branch_card_code_id
--WHERE        (dbo.branch_card_status.status_date =
--                             (SELECT        MAX(status_date) AS Expr1
--                               FROM            dbo.branch_card_status AS bcs2
--                               WHERE        (card_id = dbo.branch_card_status.card_id)))

GO
/****** Object:  View [dbo].[dist_batch_status_card_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[dist_batch_status_card_current]
AS
SELECT        dbo.cards.card_id, dbo.cards.product_id, dbo.cards.branch_id, dbo.cards.card_number, dbo.cards.card_sequence, dbo.cards.card_index, 
                         dbo.cards.card_issue_method_id, dbo.cards.card_priority_id, dbo.cards.card_request_reference, dbo.cards.card_production_date, dbo.cards.card_expiry_date, 
                         dbo.cards.card_activation_date, dbo.cards.pvv, dbo.dist_batch_cards.dist_batch_id, dbo.dist_batch_cards.card_id AS Expr1, 
                         dbo.dist_batch_cards.dist_card_status_id, dbo.dist_batch_status.dist_batch_id AS Expr2, 
                         dbo.dist_batch_status.dist_batch_statuses_id, dbo.dist_batch_status.user_id, dbo.dist_batch_status.status_date, dbo.dist_batch_status.status_notes
FROM            dbo.cards INNER JOIN
                         dbo.dist_batch_cards ON dbo.cards.card_id = dbo.dist_batch_cards.card_id INNER JOIN
                         dbo.dist_batch_status ON dbo.dist_batch_cards.dist_batch_id = dbo.dist_batch_status.dist_batch_id
WHERE        (dbo.dist_batch_status.status_date =
                             (SELECT        MAX(bcs2.status_date) AS Expr1
                               FROM            dbo.dist_batch_status AS bcs2 INNER JOIN
                                                         dbo.dist_batch_cards AS dbc2 ON bcs2.dist_batch_id = dbc2.dist_batch_id
                               WHERE        (dbc2.card_id = dbo.cards.card_id)))
GO
/****** Object:  View [dbo].[dist_batch_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[dist_batch_status_current]
AS
SELECT        dist_batch_id, dist_batch_statuses_id, user_id, status_date, status_notes
FROM            dbo.dist_batch_status
GO
/****** Object:  View [dbo].[export_batch_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[export_batch_status_current]
AS
SELECT        export_batch_id, export_batch_statuses_id, user_id, status_date, comments
FROM            dbo.export_batch_status
GO
/****** Object:  View [dbo].[hybrid_request_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[hybrid_request_status_current]
AS
SELECT        dbo.hybrid_request_status.request_id, dbo.hybrid_requests.card_priority_id, dbo.hybrid_requests.product_id, dbo.hybrid_requests.card_issue_method_id, dbo.hybrid_requests.branch_id, 
                         dbo.hybrid_request_status.hybrid_request_statuses_id, dbo.hybrid_request_status.status_date, dbo.hybrid_request_status.user_id, dbo.hybrid_request_status.operator_user_id, 
                         dbo.hybrid_requests.delivery_branch_id, dbo.hybrid_request_status.comments
FROM            dbo.hybrid_requests INNER JOIN
                         dbo.hybrid_request_status ON dbo.hybrid_requests.request_id = dbo.hybrid_request_status.request_id

GO
/****** Object:  View [dbo].[load_batch_status_card_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[load_batch_status_card_current]
AS
SELECT        dbo.cards.card_id, dbo.cards.product_id, dbo.cards.branch_id, dbo.cards.card_number, dbo.cards.card_sequence, dbo.cards.card_index, 
                         dbo.cards.card_issue_method_id, dbo.cards.card_priority_id, dbo.cards.card_request_reference, 
                         dbo.load_batch_cards.load_batch_id, dbo.load_batch_cards.load_card_status_id, 
                         dbo.load_batch_status.load_batch_statuses_id, dbo.load_batch_status.user_id, dbo.load_batch_status.status_date, 
                         dbo.load_batch_status.status_notes
FROM            dbo.cards INNER JOIN
                         dbo.load_batch_cards ON dbo.cards.card_id = dbo.load_batch_cards.card_id INNER JOIN
                         dbo.load_batch_status ON dbo.load_batch_cards.load_batch_id = dbo.load_batch_status.load_batch_id
WHERE        (dbo.load_batch_status.status_date =
                             (SELECT        MAX(bcs2.status_date) AS Expr1
                               FROM            dbo.load_batch_status AS bcs2 INNER JOIN
                                                         dbo.load_batch_cards AS dbc2 ON bcs2.load_batch_id = dbc2.load_batch_id
                               WHERE        (dbc2.card_id = dbo.cards.card_id)))




GO
/****** Object:  View [dbo].[load_batch_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[load_batch_status_current]
AS
SELECT        load_batch_id, load_batch_statuses_id, user_id, status_date, status_notes
FROM            dbo.load_batch_status
GO
/****** Object:  View [dbo].[pin_batch_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pin_batch_status_current]
AS
SELECT        pin_batch_id, pin_batch_statuses_id, user_id, status_date, status_notes
FROM            dbo.pin_batch_status
GO
/****** Object:  View [dbo].[pin_mailer_reprint_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[pin_mailer_reprint_status_current]
AS
SELECT        dbo.pin_mailer_reprint.card_id, dbo.cards.card_priority_id,
					dbo.cards.product_id, dbo.cards.card_issue_method_id, dbo.cards.branch_id,
						 dbo.pin_mailer_reprint.pin_mailer_reprint_status_id, 
                         dbo.pin_mailer_reprint.status_date, dbo.pin_mailer_reprint.[user_id],
                         dbo.pin_mailer_reprint.comments
FROM         dbo.cards 
				INNER JOIN
				   dbo.pin_mailer_reprint ON dbo.cards.card_id = dbo.pin_mailer_reprint.card_id
GO
/****** Object:  View [dbo].[pin_reissue_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pin_reissue_status_current]
AS
SELECT         pin_reissue_id, pin_reissue_statuses_id, status_date, user_id, audit_workstation, comments
FROM            dbo.pin_reissue_status
GO
/****** Object:  View [dbo].[print_batch_status_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[print_batch_status_current]
AS
SELECT        print_batch_id, print_batch_statuses_id, user_id, status_date, status_notes
FROM            dbo.print_batch_status

GO
/****** Object:  View [dbo].[remote_update_current]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[remote_update_current]
	AS 
SELECT [dbo].[remote_update_status].card_id, [dbo].[remote_update_status].comments, [dbo].[remote_update_status].remote_component, 
	[dbo].[remote_update_status].remote_update_statuses_id, [dbo].[remote_update_status].status_date, [dbo].[remote_update_status].[user_id], [dbo].[remote_update_status].remote_updated_time
FROM [dbo].[remote_update_status]
GO
/****** Object:  View [dbo].[user_group_branch_ex_ent]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[user_group_branch_ex_ent]
AS
SELECT        branch_id, [user_id], user_role_id, [user_group].issuer_id
FROM            [user_groups_branches] INNER JOIN
                         [user_group] ON [user_groups_branches].user_group_id = [user_group].user_group_id AND [user_group].all_branch_access = 0 INNER JOIN
                         [users_to_users_groups] ON [user_group].user_group_id = [users_to_users_groups].user_group_id
UNION
SELECT        branch_id, [user_id], user_role_id, [branch].issuer_id
FROM            [branch] INNER JOIN
                         [user_group] ON [user_group].issuer_id = [branch].issuer_id AND [user_group].all_branch_access = 1 INNER JOIN
                         [users_to_users_groups] ON [user_group].user_group_id = [users_to_users_groups].user_group_id



GO
/****** Object:  View [dbo].[user_roles_branch]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[user_roles_branch]
AS
SELECT        [user_groups_branches].branch_id, [user_id], user_role_id, [user_group].issuer_id, [branch].branch_status_id
FROM            [user_groups_branches] INNER JOIN
                         [user_group] ON [user_groups_branches].user_group_id = [user_group].user_group_id AND [user_group].all_branch_access = 0 INNER JOIN
                         [users_to_users_groups] ON [user_group].user_group_id = [users_to_users_groups].user_group_id INNER JOIN
                         [branch] ON [branch].[branch_id] = [user_groups_branches].branch_id
UNION
SELECT        branch_id, [user_id], user_role_id, [branch].issuer_id, [branch].branch_status_id
FROM            [branch] INNER JOIN
                         [user_group] ON [user_group].issuer_id = [branch].issuer_id AND [user_group].all_branch_access = 1 INNER JOIN
                         [users_to_users_groups] ON [user_group].user_group_id = [users_to_users_groups].user_group_id
UNION
SELECT        branch_id, [user_id], user_role_id, [branch].issuer_id, [branch].branch_status_id
FROM            [branch], [user_group] INNER JOIN
                         [users_to_users_groups] ON [user_group].user_group_id = [users_to_users_groups].user_group_id AND EXISTS
                             (SELECT        sug.*
                               FROM            [user_group] sug
                               WHERE        sug.issuer_id = - 1 AND sug.user_group_id = [user_group].user_group_id)

GO
/****** Object:  View [dbo].[user_roles_branch_pending]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  View [dbo].[user_roles_issuer]    Script Date: 2018-05-08 08:01:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[user_roles_issuer]
AS
SELECT        [issuer].issuer_id, [user_id], user_role_id, [issuer].issuer_status_id
FROM            [issuer], [user_group] INNER JOIN
                         [users_to_users_groups] ON [user_group].user_group_id = [users_to_users_groups].user_group_id AND [user_group].issuer_id = - 1
UNION
SELECT        [user_group].issuer_id, [user_id], user_role_id, [issuer].issuer_status_id
FROM            [user_group] INNER JOIN
                         [users_to_users_groups] ON [user_group].user_group_id = [users_to_users_groups].user_group_id AND [user_group].issuer_id >= 0 INNER JOIN
                         [issuer] ON [issuer].issuer_id = [user_group].issuer_id

GO