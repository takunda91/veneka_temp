-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_file_historys] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@date_from datetimeoffset,
	@date_to datetimeoffset,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

   SELECT        file_history.file_id, file_history.issuer_id, file_history.file_status_id, file_history.file_type_id, file_history.name_of_file,  CAST(SWITCHOFFSET(file_history.file_created_date,@UserTimezone) AS datetime) as file_created_date, file_history.file_size,CAST(SWITCHOFFSET(file_history.load_date,@UserTimezone) AS datetime) AS load_date , 
                         file_history.file_directory, file_history.number_successful_records, file_history.number_failed_records, file_history.file_load_comments, file_history.file_load_id, file_statuses.file_status, file_types.file_type, 
                         issuer.issuer_name, issuer.issuer_code
FROM            file_history INNER JOIN
                         file_statuses ON file_history.file_status_id = file_statuses.file_status_id INNER JOIN
                         file_types ON file_history.file_type_id = file_types.file_type_id LEFT OUTER JOIN
                         issuer ON file_history.issuer_id = issuer.issuer_id
	WHERE [file_history].issuer_id = COALESCE(@issuer_id, [file_history].issuer_id)
		  AND  SWITCHOFFSET(load_date,@UserTimezone)  BETWEEN @date_from AND @date_to  
		  END