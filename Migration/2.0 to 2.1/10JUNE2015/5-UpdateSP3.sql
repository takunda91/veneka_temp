USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_dist_batches_for_user]    Script Date: 2015-06-10 03:39:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 26 March 2014
-- Description:	Gets a list of distribution batches for a user
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_dist_batches_for_user] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@issuerId int =null,
	@dist_batch_reference VARCHAR(25) = NULL,
	@dist_batch_statuses_id int = NULL,
	@branch_id INT = NULL,
	@card_issue_method_id int = NULL,
	@distBatchTypeId int = null,
	@date_start DateTime = NULL,
	@date_end DateTime = NULL,
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

	BEGIN TRANSACTION [GET_DIST_BATCH_FOR_USER_TRAN]
		BEGIN TRY 
			IF(@dist_batch_reference = '' or @dist_batch_reference IS NULL)
				SET @dist_batch_reference = ''

			IF @date_end IS NOT NULL
				SET @date_end = DATEADD(DAY, 1, @date_end)

			DECLARE @StartRow INT, @EndRow INT;			

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY date_created DESC, dist_batch_reference ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT Distinct [dist_batch].dist_batch_id, [dist_batch].date_created, [dist_batch].dist_batch_reference, 
					   [dist_batch].no_cards, [dist_batch_status_current].status_notes,
					   [dist_batch_status_current].dist_batch_statuses_id, [dist_batch_statuses_language].language_text AS 'dist_batch_status_name', 
					   [issuer].[issuer_id], [issuer].issuer_name, [issuer].issuer_code, [issuer].auto_create_dist_batch,
					   [card_issue_method_language].language_text AS 'card_issue_method_name',
					   [dist_batch].card_issue_method_id, [dist_batch].dist_batch_type_id,
					   [dist_batch_statuses_flow].flow_dist_batch_statuses_id, [dist_batch_statuses_flow].flow_dist_batch_type_id,
					   [dist_batch_statuses_flow].user_role_id,
					   [dist_batch_statuses_flow].reject_dist_batch_statuses_id,
					   ISNULL([branch].branch_name, '-') as branch_name, 
					   ISNULL([branch].branch_code,'') as branch_code, issuer_product.product_name as 'product_name',
					   '' as [username]
				FROM [dist_batch] 
					INNER JOIN [dist_batch_status_current]
						ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
					INNER JOIN [dist_batch_statuses_language]
						ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
							AND [dist_batch_statuses_language].language_id = @language_id
					--INNER JOIN [user_roles_branch]
					--	ON [dist_batch].branch_id = [user_roles_branch].branch_id
					--		AND [user_roles_branch].user_role_id IN ( 2, 4, 5, 11, 12 )								
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [dist_batch].issuer_id
					INNER JOIN [card_issue_method_language]
						ON [dist_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
							AND [card_issue_method_language].language_id = @language_id		

					LEFT OUTER JOIN [branch]
						ON [branch].branch_id = [dist_batch].branch_id	
					LEFT OUTER JOIN [dist_batch_statuses_flow]
						ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_flow].dist_batch_statuses_id
							AND [dist_batch].card_issue_method_id = [dist_batch_statuses_flow].card_issue_method_id
							AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id	
				INNER JOIN [dist_batch_cards]
					 on [dist_batch].dist_batch_id=[dist_batch_cards].dist_batch_id
				 INNER JOIN [cards] 
					ON [dist_batch_cards].card_id=cards.card_id
				 INNER JOIN issuer_product 
					 ON cards.product_id = issuer_product.product_id
				WHERE 
					(([dist_batch].branch_id IS NULL 
						AND [dist_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @user_id))
					OR 
					  ([dist_batch].branch_id IS NOT NULL 
						AND ([dist_batch].branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @user_id)
								OR
								[cards].origin_branch_id IN(SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (2) AND [user_id] = @user_id))))
					--[user_roles_branch].[user_id] = @user_id
					AND ([dist_batch].dist_batch_reference like '%'+@dist_batch_reference+'%')
					--AND [dist_batch].dist_batch_reference LIKE COALESCE(@dist_batch_reference, [dist_batch].dist_batch_reference)
					AND [dist_batch_status_current].dist_batch_statuses_id = COALESCE(@dist_batch_statuses_id, [dist_batch_status_current].dist_batch_statuses_id)
					AND [dist_batch].card_issue_method_id = COALESCE(@card_issue_method_id, [dist_batch].card_issue_method_id)
					AND [dist_batch].dist_batch_type_id = COALESCE(@distBatchTypeId, [dist_batch].dist_batch_type_id)
					AND [dist_batch].date_created >= COALESCE(@date_start, [dist_batch].date_created)
					AND [dist_batch].date_created <= COALESCE(@date_end, [dist_batch].date_created)
					--AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
					--AND [branch].branch_status_id = 0	 
					AND [issuer].issuer_status_id = 0
					AND [issuer].issuer_id = COALESCE(@issuerId,  [issuer].issuer_id)
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY date_created DESC, dist_batch_reference ASC

			--log the audit record		
			--EXEC sp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting Distribution batches for user.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [GET_DIST_BATCH_FOR_USER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH_FOR_USER_TRAN]
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







