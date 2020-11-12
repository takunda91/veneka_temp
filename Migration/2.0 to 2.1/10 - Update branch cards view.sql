USE [indigo_database_main_dev]
GO

/****** Object:  View [dbo].[branch_card_status_current]    Script Date: 2014/08/21 03:42:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[branch_card_status_current]
AS
SELECT        dbo.branch_card_status.branch_card_status_id, dbo.branch_card_status.card_id, dbo.cards.card_priority_id,
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
WHERE        (dbo.branch_card_status.status_date =
                             (SELECT        MAX(status_date) AS Expr1
                               FROM            dbo.branch_card_status AS bcs2
                               WHERE        (card_id = dbo.branch_card_status.card_id)))



GO


