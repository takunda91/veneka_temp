CREATE PROCEDURE [dbo].[usp_get_threedsecure_batch] 
	
	@threed_batch_id bigint,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
SELECT 
				DISTINCT
				CAST(1 as BIGINT) as ROW_NO
				, 1 AS TOTAL_ROWS
				, 1 as TOTAL_PAGES
				,[threed_secure_batch_statuses_language].language_text as 'batch_status'
				 ,[threed_secure_batch].threed_batch_id
				, cast(SWITCHOFFSET([threed_secure_batch].date_created,@UserTimezone) as datetime) as 'date_created'
				, [threed_secure_batch_status].threed_batch_statuses_id
				, [threed_secure_batch].no_cards
				,[threed_secure_batch_status].status_note
				, [threed_secure_batch].batch_reference
				,'' as issuer_name
				,[threed_secure_batch].issuer_id
				--, branch.branch_code as 'BranchCode'
			FROM [threed_secure_batch] 
			INNER JOIN [threed_secure_batch_status] ON [threed_secure_batch_status].threed_batch_id = [threed_secure_batch].threed_batch_id
			INNER JOIN [threed_secure_batch_statuses_language] ON 
						[threed_secure_batch_status].threed_batch_statuses_id = [threed_secure_batch_statuses_language].threed_batch_statuses_id
						AND [threed_secure_batch_statuses_language].language_id = @language_id
			where [threed_secure_batch].threed_batch_id=@threed_batch_id
END