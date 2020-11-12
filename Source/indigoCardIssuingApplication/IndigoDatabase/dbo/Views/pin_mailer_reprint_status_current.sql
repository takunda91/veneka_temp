

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
