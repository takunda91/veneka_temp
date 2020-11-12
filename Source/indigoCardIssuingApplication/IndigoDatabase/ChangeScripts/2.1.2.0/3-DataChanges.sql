UPDATE [dbo].[cards]
SET [dbo].[cards].[ordering_branch_id] = [dbo].[cards].[branch_id],
	[dbo].[cards].[delivery_branch_id] = [dbo].[cards].[branch_id]
GO

--Set previous records to current cards branch... before upgrade some of these may be incorrect
UPDATE [dbo].[branch_card_status]
SET [branch_card_status].branch_id = [cards].branch_id
FROM [dbo].[branch_card_status] INNER JOIN [dbo].[cards] 
	ON [branch_card_status].card_id = [cards].card_id
GO

--Update product. NOTE please check those that need to use EMP!!!!
UPDATE [issuer_product]
SET production_dist_batch_status_flow = dist_batch_status_flow_id
FROM [issuer_product] INNER JOIN [dist_batch_status_flow]
	ON [dist_batch_status_flow].card_issue_method_id = [issuer_product].card_issue_method_id
		AND [dist_batch_status_flow].dist_batch_type_id = 0
		AND dist_batch_status_flow_id IN (2, 3)
GO
UPDATE [issuer_product]
SET distribution_dist_batch_status_flow = dist_batch_status_flow_id
FROM [issuer_product] INNER JOIN [dist_batch_status_flow]
	ON [dist_batch_status_flow].card_issue_method_id = [issuer_product].card_issue_method_id
		AND [dist_batch_status_flow].dist_batch_type_id = 1
		AND dist_batch_status_flow_id IN (5, 6)
GO
UPDATE [issuer_product]
	SET charge_fee_to_issuing_branch_YN = 0
GO

--UPDATE dist_batch_statuses_flow
--SET dist_batch_status_flow_id = 4
--WHERE issuer_id = 18 AND dist_batch_type_id = 0 AND card_issue_method_id = 1

--UPDATE dist_batch_statuses_flow
--SET dist_batch_status_flow_id = 11
--WHERE issuer_id = 9 AND dist_batch_type_id = 1 AND card_issue_method_id = 1

--UPDATE dist_batch_statuses_flow
--SET dist_batch_status_flow_id = 5
--WHERE issuer_id = -1 AND dist_batch_type_id = 1 AND card_issue_method_id = 0

--UPDATE dist_batch_statuses_flow
--SET dist_batch_status_flow_id = 2
--WHERE issuer_id = -1 AND dist_batch_type_id = 0 AND card_issue_method_id = 0

--UPDATE dist_batch_statuses_flow
--SET dist_batch_status_flow_id = 3
--WHERE issuer_id = -1 AND dist_batch_type_id = 0 AND card_issue_method_id = 1