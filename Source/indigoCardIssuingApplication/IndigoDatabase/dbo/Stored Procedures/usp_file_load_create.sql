-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Insert new file load
-- =============================================
CREATE PROCEDURE [dbo].[usp_file_load_create] 
	@file_load_start DATETIMEoffset, 
	@file_load_end DATETIMEoffset = NULL, 
	@user_id BIGINT, 
	@files_to_process int, 
	@audit_user_id BIGINT, 
	@audit_workstation VARCHAR(100),
	@file_load_id INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [file_load]
           ([file_load_start], [file_load_end], [user_id], [files_to_process])
     VALUES
			(@file_load_start, @file_load_end, @user_id, @files_to_process)

	SET @file_load_id = SCOPE_IDENTITY();
END