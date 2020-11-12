Create FUNCTION [dbo].[PrintBatchInCorrectStatus]
(
	@print_batch_statuses_id int,
	@new_print_batch_statuses_id int,
	@print_batch_id int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @current_print_batch_status_id int,
			@flow_from_print_batch_status_id int,
			@print_batch_type_id int,
			@card_issue_method_id int,
			@Result bit

	SET @Result = 0
	
	--get the current status for the distribution batch
	SELECT @current_print_batch_status_id = print_batch_statuses_id
	FROM print_batch_status_current
		INNER JOIN print_batch
			ON print_batch_status_current.print_batch_id = print_batch.print_batch_id
	WHERE print_batch_status_current.print_batch_id = @print_batch_id

	IF(@new_print_batch_statuses_id = @current_print_batch_status_id)
	BEGIN
			SET @Result = 1
	END
		
	return @Result
END