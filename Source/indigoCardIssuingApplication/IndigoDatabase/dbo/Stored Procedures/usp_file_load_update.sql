-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert new file load
-- =============================================
CREATE PROCEDURE [dbo].[usp_file_load_update]
	@file_load_id INT,
	@file_load_end DATETIMEOFFSET, 
	@audit_user_id BIGINT, 
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [file_load]
		SET [file_load_end] = @file_load_end
   WHERE file_load_id = @file_load_id

END