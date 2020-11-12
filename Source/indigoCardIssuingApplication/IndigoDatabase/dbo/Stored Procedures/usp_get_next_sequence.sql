-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_next_sequence] 
	-- Add the parameters for the stored procedure here
	@sequence_name varchar(100),
	@reset_period int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_SEQUENCE_TRAN]
	BEGIN TRY 

		--Reset period
		-- 0 = None
		-- 1 = Daily
		-- 2 = Weekly
		-- 3 = Monthly
		-- 4 = Yearly
		UPDATE [sequences]
			SET last_sequence_number = 0,
				last_updated = SYSDATETIMEOFFSET()
		WHERE sequence_name = @sequence_name AND
				( (@reset_period = 1 AND DATEDIFF(day, last_updated, SYSDATETIMEOFFSET()) != 0) OR
				  (@reset_period = 2 AND DATEDIFF(week, last_updated, SYSDATETIMEOFFSET()) != 0) OR
				  (@reset_period = 3 AND DATEDIFF(month, last_updated, SYSDATETIMEOFFSET()) != 0) OR
				  (@reset_period = 4 AND DATEDIFF(year, last_updated, SYSDATETIMEOFFSET()) != 0) )

		DECLARE @last_seq_number bigint,
				@next_seq_number bigint

		-- Insert statements for procedure here
		SELECT @last_seq_number = last_sequence_number
		FROM [sequences]
		WHERE sequence_name = @sequence_name


		IF(@last_seq_number IS NOT NULL)
		BEGIN
			SET @next_seq_number = @last_seq_number + 1

			UPDATE [sequences]
			SET last_sequence_number = @next_seq_number,
				last_updated = SYSDATETIMEOFFSET()
			WHERE sequence_name = @sequence_name
		END
		ELSE
		BEGIN
			SET @next_seq_number = 1

			INSERT INTO [sequences] (sequence_name, last_sequence_number, last_updated)
				VALUES (@sequence_name, @next_seq_number, SYSDATETIMEOFFSET())
		END

		COMMIT TRANSACTION [GET_SEQUENCE_TRAN]

		SELECT @next_seq_number

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_LOAD_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 
END