-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[PinBatchInCorrectStatusReject] 
(
	-- Add the parameters for the function here
	@new_pin_batch_statuses_id int,
	@pin_batch_id int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_pin_batch_status_id int,
			@flow_from_pin_batch_status_id int,
			@pin_batch_type_id int,
			@card_issue_method_id int,
			@Result bit

	SET @Result = 0

	--get the current status for the pin batch
	SELECT @current_pin_batch_status_id = pin_batch_statuses_id,
			@pin_batch_type_id = pin_batch_type_id,
			@card_issue_method_id = card_issue_method_id
	FROM pin_batch_status_current
		INNER JOIN pin_batch
			ON pin_batch_status_current.pin_batch_id = pin_batch.pin_batch_id
	WHERE pin_batch_status_current.pin_batch_id = @pin_batch_id

	--Check which status the batch must be in to flow to the new status
	SELECT @flow_from_pin_batch_status_id = pin_batch_statuses_id
	FROM pin_batch_statuses_flow
	WHERE reject_pin_batch_statuses_id = @new_pin_batch_statuses_id
		AND pin_batch_type_id = @pin_batch_type_id
		AND card_issue_method_id = @card_issue_method_id


	IF(@flow_from_pin_batch_status_id = @current_pin_batch_status_id)
		SET @Result = 1

	-- Return the result of the function
	RETURN @Result

END