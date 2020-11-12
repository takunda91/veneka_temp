-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_export_batch] 
	-- Add the parameters for the stored procedure here
	@export_batch_id bigint, 
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
    SELECT [export_batch].*,
				[export_batch_statuses_language].language_text as export_batch_statuses_name,
				[export_batch_status_current].export_batch_statuses_id, 
				[export_batch_status_current].comments,			
				CAST(SWITCHOFFSET([export_batch_status_current].status_date,@UserTimezone) AS DATETIME) as status_date	,
				[issuer].issuer_name,
				[issuer].issuer_code,
				0 as TOTAL_ROWS, 
				CONVERT(bigint, 0) AS ROW_NO,
				0 as TOTAL_PAGES				
		FROM [export_batch]
			INNER JOIN [export_batch_status_current]
				ON [export_batch].export_batch_id = [export_batch_status_current].export_batch_id
			INNER JOIN [export_batch_statuses_language]
				ON [export_batch_statuses_language].export_batch_statuses_id = [export_batch_status_current].export_batch_statuses_id
					AND [export_batch_statuses_language].language_id = @language_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [export_batch].issuer_id
		WHERE [export_batch].export_batch_id = @export_batch_id
END