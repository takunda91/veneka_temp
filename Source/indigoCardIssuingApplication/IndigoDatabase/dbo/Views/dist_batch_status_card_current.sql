
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