-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[DistBatchInCorrectStatus] 
(
	-- Add the parameters for the function here
	@dist_batch_statuses_id int,
	@new_dispatch_dist_batch_statuses_id int,
	@dist_batch_id int,
	@user_id bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_dist_batch_status_id int,
			@flow_from_dist_batch_status_id int,
			@dist_batch_status_flow_id int,			
			@Result bit

	SET @Result = 0

	--get the current status for the distribution batch
	SELECT @current_dist_batch_status_id = [dist_batch_status_current].dist_batch_statuses_id, 
			@dist_batch_status_flow_id = [product_flow].dist_batch_status_flow_id
			--, @dist_batch_type_id = dist_batch_type_id,
			--, @card_issue_method_id = card_issue_method_id
	FROM [dist_batch_status_current]
		INNER JOIN [dist_batch]
			ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
		INNER JOIN [dist_batch_cards]
			ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
		INNER JOIN cards  
			ON [dist_batch_cards].card_id = cards.card_id
		INNER JOIN [issuer_product]
			ON cards.product_id = [issuer_product].product_id
		INNER JOIN [dist_batch_statuses_flow] AS [product_flow]
			ON (([dist_batch].dist_batch_type_id = 0 AND 
					[dist_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @user_id) AND
					[product_flow].dist_batch_status_flow_id = [issuer_product].production_dist_batch_status_flow)
				OR ([dist_batch].dist_batch_type_id = 1 AND 
					([dist_batch].branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @user_id)
						OR
					 [dist_batch].origin_branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (2, 4) AND [user_id] = @user_id)) AND
					[product_flow].dist_batch_status_flow_id = [issuer_product].distribution_dist_batch_status_flow))
				AND [product_flow].dist_batch_statuses_id = [dist_batch_status_current].dist_batch_statuses_id
	WHERE dist_batch_status_current.dist_batch_id = @dist_batch_id

	--If we arent moving to a new status make sure the batch is currently in the same status
	IF(@dist_batch_statuses_id = @new_dispatch_dist_batch_statuses_id)
	BEGIN
		IF(@new_dispatch_dist_batch_statuses_id = @current_dist_batch_status_id)
			SET @Result = 1
	END
	ELSE IF(@dist_batch_statuses_id = @current_dist_batch_status_id)
	BEGIN
		--Check which status the batch must be in to flow to the new status
		SELECT @flow_from_dist_batch_status_id = dist_batch_statuses_id
		FROM dist_batch_statuses_flow
		WHERE dist_batch_status_flow_id = @dist_batch_status_flow_id
		    AND flow_dist_batch_statuses_id = @new_dispatch_dist_batch_statuses_id
			AND dist_batch_statuses_id = @dist_batch_statuses_id
			--AND dist_batch_type_id = @dist_batch_type_id
			--AND card_issue_method_id = @card_issue_method_id


		IF(@flow_from_dist_batch_status_id = @current_dist_batch_status_id)
			SET @Result = 1
	END

	-- Return the result of the function
	RETURN @Result

END