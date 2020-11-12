-- =============================================
-- Author:		Richard Brenchey
-- Create date: 5 March 2014
-- Description:	Find file info based on filename.
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_file_info_by_filename]
	-- Add the parameters for the stored procedure here
	@filename varchar(60) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [file_history].*
	FROM [file_history]
		INNER JOIN [load_batch]
			ON [load_batch].[file_id] =[file_history].[file_id]
		INNER JOIN [load_batch_status_current]
			ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
	WHERE [file_history].name_of_file = @filename
		  AND [load_batch_status_current].load_batch_statuses_id NOT IN (2, 3)

END