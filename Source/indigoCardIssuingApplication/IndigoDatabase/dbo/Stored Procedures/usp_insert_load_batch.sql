-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert into load_batch table and returns the id of the inserted row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_load_batch] 
	@load_batch_reference varchar(50),
	@file_id bigint,
	@issuer_id int,
	@load_batch_status_id int,
	@load_date datetimeoffset,
	@number_of_cards int,
	@load_batch_type_id int,
	@load_batch_id bigint = NULL OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[load_batch]
			([load_batch_reference],[file_id],[load_batch_status_id],[load_date],[no_cards], [load_batch_type_id])
	VALUES
			(@load_batch_reference, @file_id, @load_batch_status_id, @load_date, @number_of_cards, @load_batch_type_id)

	SET @load_batch_id = SCOPE_IDENTITY();
END