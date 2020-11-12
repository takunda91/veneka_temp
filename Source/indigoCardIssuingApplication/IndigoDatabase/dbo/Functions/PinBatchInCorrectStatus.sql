
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[PinBatchInCorrectStatus] 
(
	@pin_batch_statuses_id int,
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

	--get the current status for the distribution batch
	SELECT @current_pin_batch_status_id = pin_batch_statuses_id,
			@pin_batch_type_id = pin_batch_type_id,
			@card_issue_method_id = card_issue_method_id
	FROM pin_batch_status_current
		INNER JOIN pin_batch
			ON pin_batch_status_current.pin_batch_id = pin_batch.pin_batch_id
	WHERE pin_batch_status_current.pin_batch_id = @pin_batch_id


	IF(@pin_batch_statuses_id = @new_pin_batch_statuses_id)
	BEGIN
		IF(@new_pin_batch_statuses_id = @current_pin_batch_status_id)
			SET @Result = 1
	END
	ELSE IF(@pin_batch_statuses_id = @current_pin_batch_status_id)
	BEGIN
		--Check which status the batch must be in to flow to the new status
		SELECT @flow_from_pin_batch_status_id = pin_batch_statuses_id
		FROM pin_batch_statuses_flow
		WHERE flow_pin_batch_statuses_id = @new_pin_batch_statuses_id
			AND pin_batch_statuses_id = @pin_batch_statuses_id
			AND pin_batch_type_id = @pin_batch_type_id
			AND card_issue_method_id = @card_issue_method_id


		IF(@flow_from_pin_batch_status_id = @current_pin_batch_status_id)
			SET @Result = 1
	END

	-- Return the result of the function
	RETURN @Result

END