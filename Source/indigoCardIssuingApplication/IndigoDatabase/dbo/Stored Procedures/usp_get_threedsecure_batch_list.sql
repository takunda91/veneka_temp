-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Returns a single load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_threedsecure_batch_list] 
	@issuer_id int,
	@datefrom datetime,
	@dateto datetime,
	@threed_batch_statuses_id int,
	@batch_reference nvarchar,
	@language_id int ,
	@PageIndex int,
	@RowsPerPage int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	BEGIN TRANSACTION [GET_THREED_BATCH]
		BEGIN TRY 
		IF(@batch_reference = '' or @batch_reference IS NULL)
				SET @batch_reference = ''
DECLARE @StartRow INT, @EndRow INT;			

	SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
	SET @EndRow = @StartRow + @RowsPerPage - 1;

	WITH PAGE_ROWS
	AS
	(
	SELECT ROW_NUMBER() OVER(ORDER BY issuer_name ASC) AS ROW_NO
			, COUNT(*) OVER() AS TOTAL_ROWS
			, *
	FROM(
			SELECT 
				DISTINCT
				[threed_secure_batch_statuses_language].language_text as 'batch_status'
				 ,[threed_secure_batch].threed_batch_id
				, cast(SWITCHOFFSET([threed_secure_batch].date_created,@UserTimezone) as datetime) as 'date_created'
				, [threed_secure_batch_status].threed_batch_statuses_id
				--, [threed_secure_batch].no_cards
				,no_cards 
				, [threed_secure_batch].batch_reference
				,'' as issuer_name,[threed_secure_batch_status].status_note,[threed_secure_batch].issuer_id
				--, branch.branch_code as 'BranchCode'
			FROM [threed_secure_batch] 
			INNER JOIN [threed_secure_batch_status] ON [threed_secure_batch_status].threed_batch_id = [threed_secure_batch].threed_batch_id
			INNER JOIN [threed_secure_batch_statuses_language] ON 
						[threed_secure_batch_status].threed_batch_statuses_id = [threed_secure_batch_statuses_language].threed_batch_statuses_id
						AND [threed_secure_batch_statuses_language].language_id = @language_id
			--INNER JOIN [threed_secure_batch_cards] ON threed_secure_batch_cards.threed_batch_id = threed_secure_batch.threed_batch_id
			--INNER JOIN cards ON cards.card_id = threed_secure_batch_cards.card_id
			--INNER JOIN branch ON branch.branch_id = cards.branch_id	
			--INNER JOIN  issuer ON branch.issuer_id = issuer.issuer_id
			WHERE 
			 ([threed_secure_batch].batch_reference like '%'+@batch_reference+'%')
			AND date_created >= COALESCE(@datefrom, date_created)
					AND date_created <= COALESCE(@dateto, date_created)
					--AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
					--AND [branch].branch_status_id = 0	 
					--AND [issuer].issuer_status_id = 0
					--AND [issuer].issuer_id = COALESCE(@issuer_id,  [issuer].issuer_id)	
				
				 AND [threed_secure_batch_status].threed_batch_statuses_id = COALESCE(@threed_batch_statuses_id, [threed_secure_batch_status].threed_batch_statuses_id)	
	
				 
					

	)AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY issuer_name ASC
			--DECLARE @audit_msg varchar
			--SET @audit_msg = 'Getting Load batch with id: ' + CAST(@load_batch_id AS varchar(max))
			----log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 @audit_msg, 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_THREED_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_THREED_BATCH]
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