USE [indigo_database_main_dev]
GO
/****** Object:  UserDefinedFunction [dbo].[DistBatchInCorrectStatus]    Script Date: 2015-05-05 03:40:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
ALTER FUNCTION [dbo].[DistBatchInCorrectStatus] 
(
	-- Add the parameters for the function here
	@dist_batch_statuses_id int,
	@new_dispatch_dist_batch_statuses_id int,
	@dist_batch_id int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_dist_batch_status_id int,
			@flow_from_dist_batch_status_id int,
			@dist_batch_type_id int,
			@card_issue_method_id int,
			@Result bit

	SET @Result = 0

	--get the current status for the distribution batch
	SELECT @current_dist_batch_status_id = dist_batch_statuses_id,
			@dist_batch_type_id = dist_batch_type_id,
			@card_issue_method_id = card_issue_method_id
	FROM dist_batch_status_current
		INNER JOIN dist_batch
			ON dist_batch_status_current.dist_batch_id = dist_batch.dist_batch_id
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
		WHERE flow_dist_batch_statuses_id = @new_dispatch_dist_batch_statuses_id
			AND dist_batch_statuses_id = @dist_batch_statuses_id
			AND dist_batch_type_id = @dist_batch_type_id
			AND card_issue_method_id = @card_issue_method_id


		IF(@flow_from_dist_batch_status_id = @current_dist_batch_status_id)
			SET @Result = 1
	END

	-- Return the result of the function
	RETURN @Result

END
