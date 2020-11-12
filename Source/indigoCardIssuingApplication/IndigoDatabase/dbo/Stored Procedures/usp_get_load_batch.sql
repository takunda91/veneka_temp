-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Returns a single load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_load_batch] 
	@load_batch_id bigint,
	@language_id INT,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	BEGIN TRANSACTION [GET_LOAD_BATCH]
		BEGIN TRY 

			SELECT 
				DISTINCT
				CAST(1 as BIGINT) as ROW_NO
				, 1 AS TOTAL_ROWS
				, 1 as TOTAL_PAGES
				, [load_batch].load_batch_id
				, cast(SWITCHOFFSET([load_batch].load_date,@UserTimezone) as datetime) as 'load_date'
				, [load_batch].load_batch_reference
				, [load_batch].no_cards
				, [load_batch_status].load_batch_statuses_id
				, [load_batch_status].status_notes
				, [load_batch_statuses_language].language_text AS 'load_batch_status_name'
				, branch.branch_name as 'BranchName'
				--, branch.branch_code as 'BranchCode'
			FROM [load_batch] 
			INNER JOIN [load_batch_status] ON [load_batch_status].load_batch_id = [load_batch].load_batch_id
			INNER JOIN [load_batch_statuses_language] ON 
						[load_batch_status].load_batch_statuses_id = [load_batch_statuses_language].load_batch_statuses_id	
						AND [load_batch_statuses_language].language_id = @language_id
			INNER JOIN [load_batch_cards] ON load_batch_cards.load_batch_id = load_batch.load_batch_id
			INNER JOIN cards ON cards.card_id = load_batch_cards.card_id
			INNER JOIN branch ON branch.branch_id = cards.branch_id	
			WHERE 
				[load_batch].load_batch_id = @load_batch_id	
				AND  SWITCHOFFSET([load_batch_status].status_date,@UserTimezone)  = (SELECT MAX(lbs2.status_date)
													   FROM [load_batch_status] lbs2
													   WHERE lbs2.load_batch_id = [load_batch].load_batch_id)		

			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting Load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_LOAD_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_LOAD_BATCH]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END