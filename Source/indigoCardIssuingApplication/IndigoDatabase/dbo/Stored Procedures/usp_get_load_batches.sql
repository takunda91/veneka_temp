CREATE PROCEDURE [dbo].[usp_get_load_batches]
	@user_id bigint,
	@load_batch_reference VARCHAR(50) = NULL,
	@issuerId int,
	@load_batch_statuses_id int = NULL,
	@date_start DateTimeoffset = NULL,
	@date_end DateTimeoffset = NULL,
	@language_id INT,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if(@issuerId=0)
	set @issuerId=null

	if(@load_batch_reference <>'' or @load_batch_reference<> null)
	set @load_batch_reference ='%'+@load_batch_reference+'%'
	
Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	BEGIN TRANSACTION [GET_LOAD_BATCHES]
		BEGIN TRY 
			
			IF @date_end IS NOT NULL
				SET @date_end = DATEADD(day, 1, @date_end)
			
			DECLARE @StartRow INT, @EndRow INT;			

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH cte AS (
				SELECT DISTINCT [load_batch].load_batch_id,cast(SWITCHOFFSET([load_batch].load_date,@UserTimezone)as datetime) as load_date  , [load_batch].load_batch_reference, 
									   [load_batch].no_cards, [load_batch_status_current].load_batch_statuses_id, 
									   [load_batch_status_current].status_notes, [load_batch_statuses_language].language_text AS 'load_batch_status_name',
									   [branch].[branch_code]+' '+ [branch].branch_name as 'BranchName'
								FROM [load_batch] 
									INNER JOIN [load_batch_status_current]
										ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
									INNER JOIN [load_batch_statuses_language]
										ON [load_batch_status_current].load_batch_statuses_id = [load_batch_statuses_language].load_batch_statuses_id
										  AND  [load_batch_statuses_language].language_id = 0
									INNER JOIN [load_batch_cards]
										ON [load_batch_cards].load_batch_id = [load_batch].load_batch_id
									INNER JOIN [cards]
										ON [cards].card_id = [load_batch_cards].card_id
									INNER JOIN [branch]
										ON [cards].branch_id = [branch].branch_id
				WHERE 
					[load_batch].load_batch_reference LIKE (COALESCE(@load_batch_reference, REPLACE([load_batch].load_batch_reference, '[', '[['))) ESCAPE '['
					AND [branch].issuer_id = COALESCE(@issuerId,[branch].issuer_id)
					AND [load_batch_status_current].load_batch_statuses_id = COALESCE(@load_batch_statuses_id, [load_batch_status_current].load_batch_statuses_id)
					AND SWITCHOFFSET([load_batch].load_date,@UserTimezone) BETWEEN COALESCE(@date_start, SWITCHOFFSET([load_batch].load_date,@UserTimezone))
													AND COALESCE(@date_end, SWITCHOFFSET([load_batch].load_date,@UserTimezone))
			),
			PAGE_ROWS	AS	(
				SELECT ROW_NUMBER() OVER(ORDER BY load_date ASC, load_batch_reference ASC) AS ROW_NO
						, COUNT(*) OVER() AS TOTAL_ROWS
						, *
				FROM( SELECT DISTINCT cte.load_batch_id, cte.load_date, cte.load_batch_reference, 
								cte.no_cards, cte.load_batch_statuses_id, cte.load_batch_status_name,
								cte.status_notes
								,CASE WHEN cte_grp.grpcnt > 1 THEN '-' ELSE cte.BranchName END as BranchName
						FROM 
							(SELECT load_batch_id, count(*) as grpcnt 
							FROM cte GROUP BY load_batch_id ) as cte_grp
							INNER JOIN cte ON cte.load_batch_id = cte_grp.load_batch_id
					) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY load_date ASC, load_batch_reference ASC

			--log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting Load batches.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_LOAD_BATCHES]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_LOAD_BATCHES]
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