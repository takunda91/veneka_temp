-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert to file_history table.
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_file_history] 	
	@file_load_id INT,
	@issuer_id int = NULL,
	@name_of_file varchar(60),
	@file_created_date datetimeoffset,
	@file_size int,
	@load_date datetimeoffset,
	@file_status_id int,
	@file_directory varchar(110),
	@number_successful_records int,
	@number_failed_records int,
	@file_load_comments varchar(max),
	@file_type_id int,
	@file_id bigint = NULL OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF(@file_id IS NULL)
		BEGIN
			INSERT INTO [dbo].[file_history]
				   ([issuer_id],[name_of_file],[file_created_date],[file_size],[load_date],[file_status_id],[file_directory],
					[number_successful_records],[number_failed_records],[file_load_comments],[file_type_id], [file_load_id])		 
			 VALUES
					(@issuer_id, @name_of_file, @file_created_date, @file_size, @load_date, @file_status_id, @file_directory,
					 @number_successful_records, @number_failed_records, @file_load_comments, @file_type_id, @file_load_id)		

			SET @file_id = SCOPE_IDENTITY();
		END
	ELSE
		BEGIN
			UPDATE [dbo].[file_history]
			SET [file_status_id] = @file_status_id,
				[number_successful_records] = @number_successful_records,
				[number_failed_records] = @number_failed_records,
				[file_load_comments] = @file_load_comments
			WHERE [file_id] = @file_id
		END
END