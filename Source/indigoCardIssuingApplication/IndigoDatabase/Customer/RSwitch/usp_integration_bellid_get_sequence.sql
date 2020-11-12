
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_bellid_get_sequence] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @seq_no smallint,
			@current_date datetime2

	SET @current_date = SYSDATETIMEOFFSET()

    -- Insert statements for procedure here
	SELECT @seq_no = batch_sequence_number
	FROM [integration_bellid_batch_sequence]
	WHERE file_generation_date = @current_date

	IF @seq_no IS NULL
		BEGIN
			--Start of a new seqence for today.
			SET @seq_no = 1
			INSERT INTO [integration_bellid_batch_sequence] (file_generation_date, batch_sequence_number)
				VALUES (@current_date, @seq_no)
		END
	ELSE
		BEGIN
			--BECAUSE there is already a sequence for today, increment it and update the table.
			SET @seq_no = @seq_no + 1
			UPDATE [integration_bellid_batch_sequence]
			SET batch_sequence_number = @seq_no
			WHERE file_generation_date = @current_date
		END

	SELECT @seq_no

END