-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Search for export batches based on filter criteria
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_export_batches] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@product_id int = null,
	@export_batch_statuses_id int = null,
	@date_from DATETIMEOFFSET = null,
	@date_to DATETIMEOFFSET = null,
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @date_to IS NOT NULL
	SET @date_to = DATEADD(day, 1, @date_to)
	Declare @UserTimezone as nvarchar(50);
	set @UserTimezone=[dbo].[GetUserTimeZone](@audit_user_id);

	SELECT [export_batch_status_current].*, 
			[export_batch].date_created,
			[export_batch].batch_reference
	FROM [export_batch]
		INNER JOIN [export_batch_status_current]
			ON [export_batch].export_batch_id = [export_batch_status_current].export_batch_id
		INNER JOIN [export_batch_statuses_language]
			ON [export_batch_statuses_language].export_batch_statuses_id = [export_batch_status_current].export_batch_statuses_id
				AND [export_batch_statuses_language].language_id = @language_id
	WHERE [export_batch].issuer_id = COALESCE(@issuer_id, [export_batch].issuer_id)
		AND SWITCHOFFSET([export_batch].date_created,@UserTimezone)  BETWEEN COALESCE(@date_from, SWITCHOFFSET([export_batch].date_created,@UserTimezone)) AND COALESCE(@date_to, SWITCHOFFSET([export_batch].date_created,@UserTimezone))
		AND	[export_batch_status_current].export_batch_statuses_id = COALESCE(@export_batch_statuses_id, [export_batch_status_current].export_batch_statuses_id)
		AND((@product_id IS NULL) OR ([export_batch].export_batch_id IN (SELECT export_batch_id FROM [cards] where product_id = @product_id)))
END