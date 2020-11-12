Create PROCEDURE [dbo].[usp_get_print_batches_for_user] 
	-- Add the parameters for the stored procedure here
	@issuerId int = NULL,
	@product_id int=null,
	@print_batch_reference VARCHAR(50) = NULL,
	@print_batch_statuses_id int = NULL,
	@branch_id INT = NULL,
	@card_issue_method_id int = NULL,
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

	BEGIN TRANSACTION [GET_print_BATCH_FOR_USER_TRAN]
		BEGIN TRY 
			IF(@print_batch_reference = '' or @print_batch_reference IS NULL)
				SET @print_batch_reference = ''
				
			IF @date_end IS NOT NULL
				SET @date_end = DATEADD(DAY, 1, @date_end)

			DECLARE @StartRow INT, @EndRow INT;			
			
			Declare @UserTimezone as nvarchar(50);
			set @UserTimezone=[dbo].[GetUserTimeZone](@audit_user_id);

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC, print_batch_reference ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT distinct [print_batch].print_batch_id, CAST(SWITCHOFFSET( [print_batch].date_created,@UserTimezone) as DateTime) as 'date_created', [print_batch].print_batch_reference, 
					  COALESCE( [print_batch].no_of_requests,0) as 'no_cards', [print_batch_status_current].status_notes,
					   [print_batch_status_current].print_batch_statuses_id, [print_batch_statuses_language].language_text AS 'print_batch_status_name', 
					   [issuer].[issuer_id], [issuer].issuer_name, [issuer].issuer_code,
					   [card_issue_method_language].language_text AS 'card_issue_method_name',
					   [print_batch].card_issue_method_id, hybrid_requests.product_id,
					   [branch].branch_name, [branch].branch_code
					   --[print_batch_statuses_flow].flow_print_batch_statuses_id, 
					   --[print_batch_statuses_flow].flow_print_batch_type_id,
					   --[print_batch_statuses_flow].user_role_id,
					   --[print_batch_statuses_flow].reject_print_batch_statuses_id 				   
				FROM [print_batch] 
				INNER JOIN [print_batch_requests] on [print_batch].print_batch_id=[print_batch_requests].print_batch_id
					INNER JOIN [print_batch_status_current]
						ON [print_batch_status_current].print_batch_id = [print_batch].print_batch_id
						inner join hybrid_requests on [print_batch_requests].request_id=hybrid_requests.request_id
					INNER JOIN [print_batch_statuses_language]
						ON [print_batch_status_current].print_batch_statuses_id = [print_batch_statuses_language].print_batch_statuses_id
							AND [print_batch_statuses_language].language_id = @language_id								
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [print_batch].issuer_id
					INNER JOIN [card_issue_method_language]
						ON [print_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id
									
					LEFT OUTER JOIN [branch]
						ON [branch].branch_id = [print_batch].branch_id
					--LEFT OUTER JOIN [print_batch_statuses_flow]
					--	ON [print_batch_status_current].print_batch_statuses_id = [print_batch_statuses_flow].print_batch_statuses_id
					--		AND [print_batch].card_issue_method_id = [print_batch_statuses_flow].card_issue_method_id
					--		AND [print_batch_statuses_flow].print_batch_type_id = [print_batch].print_batch_type_id	
				WHERE 
					(([print_batch].branch_id IS NULL 
						AND [print_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE user_role_id IN (4, 12, 13, 14, 15,18,19) AND [user_id] = @audit_user_id))
					OR 
					  ([print_batch].branch_id IS NOT NULL 
						AND [print_batch].branch_id IN (SELECT branch.branch_id FROM [user_roles_branch] inner join branch on [user_roles_branch].branch_id=branch.main_branch_id   WHERE user_role_id IN (4, 12, 13, 14, 15,18,19) AND [user_id] = @audit_user_id)))

					--AND [print_batch].print_batch_reference LIKE COALESCE(@print_batch_reference, [print_batch].print_batch_reference)
					AND ([print_batch].print_batch_reference like '%'+@print_batch_reference+'%')
					AND [print_batch_status_current].print_batch_statuses_id = COALESCE(@print_batch_statuses_id, [print_batch_status_current].print_batch_statuses_id)
					AND [print_batch].card_issue_method_id = COALESCE(@card_issue_method_id, [print_batch].card_issue_method_id)
					--AND [print_batch].print_batch_type_id = COALESCE(@print_batch_type_id, [print_batch].print_batch_type_id)
					AND SWITCHOFFSET( [print_batch].date_created,@UserTimezone)  >= COALESCE(@date_start, SWITCHOFFSET( [print_batch].date_created,@UserTimezone))
					AND SWITCHOFFSET( [print_batch].date_created,@UserTimezone) <= COALESCE(@date_end, SWITCHOFFSET( [print_batch].date_created,@UserTimezone))
					AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
					AND [branch].branch_status_id = 0	 
					AND hybrid_requests.product_id= COALESCE(@product_id, hybrid_requests.product_id)
					AND [issuer].issuer_status_id = 0
					AND [issuer].issuer_id = COALESCE(@issuerId,  [issuer].issuer_id)
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY date_created DESC, print_batch_reference ASC

			--log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting Distribution batches for user.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_print_BATCH_FOR_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_print_BATCH_FOR_USER_TRAN]
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



