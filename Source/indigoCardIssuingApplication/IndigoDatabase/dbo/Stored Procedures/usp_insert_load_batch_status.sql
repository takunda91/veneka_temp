-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Insert into load_batch_status
-- =============================================
CREATE PROCEDURE [dbo].[usp_insert_load_batch_status] 	
	@load_batch_id bigint, 
	@load_batch_statuses_id int,
	@status_date datetimeoffset,
	@user_id bigint,
	@load_batch_status_id bigint = null OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [load_batch_status]
	       ([load_batch_id],[load_batch_statuses_id],[status_date],[user_id],[status_notes])
     VALUES
		    (@load_batch_id, @load_batch_statuses_id, @status_date, @user_id, '')

	SET @load_batch_status_id = SCOPE_IDENTITY();
           
END