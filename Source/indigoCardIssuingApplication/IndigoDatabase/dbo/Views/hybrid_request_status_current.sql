CREATE VIEW [dbo].[hybrid_request_status_current]
	AS SELECT        dbo.hybrid_request_status.request_id, dbo.hybrid_requests.card_priority_id, dbo.hybrid_requests.product_id, dbo.hybrid_requests.card_issue_method_id, dbo.hybrid_requests.branch_id, 
                         dbo.hybrid_request_status.hybrid_request_statuses_id, dbo.hybrid_request_status.status_date, dbo.hybrid_request_status.user_id, dbo.hybrid_request_status.operator_user_id, 
                         dbo.hybrid_requests.delivery_branch_id, dbo.hybrid_request_status.comments
FROM            dbo.hybrid_requests INNER JOIN
                         dbo.hybrid_request_status ON dbo.hybrid_requests.request_id = dbo.hybrid_request_status.request_id
