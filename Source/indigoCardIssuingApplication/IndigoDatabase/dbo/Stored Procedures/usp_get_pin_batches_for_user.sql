-- =============================================
-- Author:		Richard Brenchley
-- Description:	Gets a list of pin batches for a user
-- =============================================

--exec usp_get_pin_batches_for_user 3,null,null,null,null,null,null,null,null,1,20,17,'veneka-13'
CREATE PROCEDURE [dbo].[usp_get_pin_batches_for_user] 
	-- Add the parameters for the stored procedure here
	@issuerId int = NULL,
	@pin_batch_reference VARCHAR(50) = NULL,
	@pin_batch_statuses_id int = NULL,
	@branch_id INT = NULL,
	@card_issue_method_id int = NULL,
	@pin_batch_type_id int = NULL,
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

	BEGIN TRANSACTION [GET_PIN_BATCH_FOR_USER_TRAN]
		BEGIN TRY 
			IF(@pin_batch_reference = '' or @pin_batch_reference IS NULL)
				SET @pin_batch_reference = ''
				
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
			SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC, pin_batch_reference ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT [pin_batch].pin_batch_id, CAST(SWITCHOFFSET( [pin_batch].date_created,@UserTimezone) as DateTime) as 'date_created', [pin_batch].pin_batch_reference, 
					   [pin_batch].no_cards, [pin_batch_status_current].status_notes,
					   [pin_batch_status_current].pin_batch_statuses_id, [pin_batch_statuses_language].language_text AS 'pin_batch_status_name', 
					   [issuer].[issuer_id], [issuer].issuer_name, [issuer].issuer_code,
					   [card_issue_method_language].language_text AS 'card_issue_method_name',
					   [pin_batch].card_issue_method_id, [pin_batch].pin_batch_type_id,
					   [branch].branch_name, [branch].branch_code,
					   [pin_batch_statuses_flow].flow_pin_batch_statuses_id, 
					   [pin_batch_statuses_flow].flow_pin_batch_type_id,
					   [pin_batch_statuses_flow].user_role_id,
					   [pin_batch_statuses_flow].reject_pin_batch_statuses_id 				   
				FROM [pin_batch] 
					INNER JOIN [pin_batch_status_current]
						ON [pin_batch_status_current].pin_batch_id = [pin_batch].pin_batch_id
					INNER JOIN [pin_batch_statuses_language]
						ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_language].pin_batch_statuses_id
							AND [pin_batch_statuses_language].language_id = @language_id								
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [pin_batch].issuer_id
					INNER JOIN [card_issue_method_language]
						ON [pin_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id
									
					LEFT OUTER JOIN [branch]
						ON [branch].branch_id = [pin_batch].branch_id
					LEFT OUTER JOIN [pin_batch_statuses_flow]
						ON [pin_batch_status_current].pin_batch_statuses_id = [pin_batch_statuses_flow].pin_batch_statuses_id
							AND [pin_batch].card_issue_method_id = [pin_batch_statuses_flow].card_issue_method_id
							AND [pin_batch_statuses_flow].pin_batch_type_id = [pin_batch].pin_batch_type_id	
				WHERE 
					(([pin_batch].branch_id IS NULL 
						AND [pin_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE user_role_id IN (4, 12, 13, 14, 15) AND [user_id] = @audit_user_id))
					OR 
					  ([pin_batch].branch_id IS NOT NULL 
						AND [pin_batch].branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (4, 12, 13, 14, 15) AND [user_id] = @audit_user_id)))

					--AND [pin_batch].pin_batch_reference LIKE COALESCE(@pin_batch_reference, [pin_batch].pin_batch_reference)
					AND ([pin_batch].pin_batch_reference like '%'+@pin_batch_reference+'%')
					AND [pin_batch_status_current].pin_batch_statuses_id = COALESCE(@pin_batch_statuses_id, [pin_batch_status_current].pin_batch_statuses_id)
					AND [pin_batch].card_issue_method_id = COALESCE(@card_issue_method_id, [pin_batch].card_issue_method_id)
					AND [pin_batch].pin_batch_type_id = COALESCE(@pin_batch_type_id, [pin_batch].pin_batch_type_id)
					AND SWITCHOFFSET( [pin_batch].date_created,@UserTimezone)  >= COALESCE(@date_start, SWITCHOFFSET( [pin_batch].date_created,@UserTimezone))
					AND SWITCHOFFSET( [pin_batch].date_created,@UserTimezone) <= COALESCE(@date_end, SWITCHOFFSET( [pin_batch].date_created,@UserTimezone))
					--AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
					--AND [branch].branch_status_id = 0	 
					AND [issuer].issuer_status_id = 0
					AND [issuer].issuer_id = COALESCE(@issuerId,  [issuer].issuer_id)
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY date_created DESC, pin_batch_reference ASC

			--log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting Distribution batches for user.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_PIN_BATCH_FOR_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH_FOR_USER_TRAN]
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
GO

