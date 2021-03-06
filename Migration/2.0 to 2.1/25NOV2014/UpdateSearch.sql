USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_cards]    Script Date: 2014/11/25 09:38:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 28 March 2014
-- Description:	Search for card/s based on input parameters
-- =============================================
ALTER PROCEDURE [dbo].[sp_search_cards] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@user_role_id int = NULL,
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@card_number varchar(30) = NULL, 
	@card_last_four_digits varchar(4) = NULL,	
	@card_refnumber varchar(50) =NULL,
	@batch_reference varchar(100) = NULL,
	@load_card_status_id int = NULL,
	@dist_card_status_id int = NULL,	
	@branch_card_statuses_id int = NULL,

	@account_number varchar(100) = NULL,
	@first_name varchar(100) = NULL,
	@last_name varchar(100) = NULL,
	@cms_id varchar(100) = NULL,	

	@date_from DATETIME = NULL,
	@date_to DATETIME = NULL,

	@card_issue_method_id int = NULL,
	@product_id int = NULL,
	@sub_product_id int = NULL,
	@priority_id int = NULL,

	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CARD_SEARCH_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			IF @date_to IS NOT NULL
				SET @date_to = DATEADD(day, 1, @date_to)

			--THIS IS FOR QUICKER CARD LOOKUP
			DECLARE @objid int
			SET @objid = object_id('cards')
			
			DECLARE @StartRow INT, @EndRow INT;			
			
			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS			
					, *
			FROM( 				
				SELECT DISTINCT [cards].card_id, 
					   --ISNULL(NULLIF(CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)), ''), 'UNASSIGNED') AS card_number,
						CASE WHEN [issuer].card_ref_preference = 1 THEN CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_number))
							 WHEN [issuer].card_ref_preference = 0 THEN [cards].card_request_reference
						END as  card_number,
					   --[load_batch].load_date, 
					   --[load_batch].load_batch_id,
					   --[load_card_statuses].load_card_status, 
					  -- [dist_batch].date_created,					   
					  --[dist_batch].dist_batch_id,
					 --  [dist_card_statuses].dist_card_status_name,
					 --  [branch_card_statuses].branch_card_statuses_name,
					   [cards].product_id, 
					   [cards].sub_product_id,
					   [cards].card_priority_id, 
					   [cards].card_issue_method_id,
					   COALESCE([branch_card_status_current].comments,
								[dist_batch_status_card_current].status_notes,
								[load_batch_status_current].status_notes,
								'') as comments,
					   COALESCE([branch_card_statuses].branch_card_statuses_name, 
								[dist_card_statuses_language].language_text,
								[load_card_statuses].load_card_status) as current_card_status,
					   COALESCE([branch_card_status_current].status_date, 
								[dist_batch_status_card_current].status_date,
								[load_batch_status_current].status_date) as status_date,
						ISNULL([branch_card_status_current].branch_card_statuses_id, 0) as branch_card_statuses_id,	
					   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
					   [branch].branch_id, [branch].branch_name, [branch].branch_code
				FROM [cards]						
					LEFT OUTER JOIN [load_batch_cards]
						ON [cards].card_id = [load_batch_cards].card_id
					LEFT OUTER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_cards].load_batch_id
					LEFT OUTER JOIN [load_batch_status_current]
						ON [load_batch].load_batch_id = [load_batch_status_current].load_batch_id
					LEFT OUTER JOIN [load_card_statuses]
						ON [load_card_statuses].load_card_status_id = [load_batch_cards].load_card_status_id
					--Dist batch joins
					LEFT OUTER JOIN [dist_batch_status_card_current]
						ON [cards].card_id = [dist_batch_status_card_current].card_id
					LEFT OUTER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_status_card_current].dist_batch_id
					LEFT OUTER JOIN [dist_card_statuses_language]
						ON [dist_card_statuses_language].dist_card_status_id = [dist_batch_status_card_current].dist_card_status_id
							AND [dist_card_statuses_language].language_id = 0

					--branch card joins
					LEFT OUTER JOIN [branch_card_status_current]
						ON [branch_card_status_current].card_id = [cards].card_id						   
					LEFT OUTER JOIN [branch_card_statuses]
						ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses].branch_card_statuses_id							

					--Link to customer
					LEFT OUTER JOIN [customer_account]
						ON [cards].card_id = [customer_account].card_id

					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT branch_id								
								FROM [user_roles_branch] INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
								WHERE [user_roles_branch].[user_id] = @user_id
										AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)
										AND [user_roles].user_role_id IN (1,2,3,4,5,7)--Only want roles that allowed to search cards
										AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
								) as X
						ON [cards].branch_id = X.branch_id
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
				WHERE  ((@batch_reference IS NULL) OR 			 
					    ([load_batch].load_batch_reference LIKE @batch_reference
							OR
					      [dist_batch].dist_batch_reference LIKE @batch_reference))	
					  AND ((@load_card_status_id IS NULL) OR ([load_batch_cards].load_card_status_id = @load_card_status_id)) 				  
					  --AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
					  AND [branch].branch_status_id = 0	 
					  AND [issuer].issuer_status_id = 0		
					  AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)		  
					  AND (([dist_batch].date_created BETWEEN COALESCE(@date_from, [dist_batch].date_created) AND COALESCE(@date_to, [dist_batch].date_created))
							OR
						   ([load_batch].load_date BETWEEN COALESCE(@date_from, [load_batch].load_date) AND COALESCE(@date_to, [load_batch].load_date))
							OR
						   ([branch_card_status_current].status_date BETWEEN COALESCE(@date_from, [branch_card_status_current].status_date) AND COALESCE(@date_to, [branch_card_status_current].status_date)))
					  AND ((@card_number IS NULL) OR (DECRYPTBYKEY([cards].card_number) LIKE @card_number))					  
					  AND ((@dist_card_status_id IS NULL) OR ([dist_batch_status_card_current].dist_card_status_id = @dist_card_status_id))
					  AND ((@branch_card_statuses_id IS NULL) OR ([branch_card_status_current].branch_card_statuses_id = @branch_card_statuses_id))
					  AND ((@card_last_four_digits IS NULL) OR ([cards].[card_index] = [dbo].[MAC] (@card_last_four_digits, @objid)))
					  AND [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id)
					  AND [cards].product_id = COALESCE(@product_id, [cards].product_id)
					  AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
					  AND [cards].card_request_reference = COALESCE(@card_refnumber, [cards].card_request_reference)
					  AND [cards].card_priority_id = COALESCE(@priority_id, [cards].card_priority_id)
					  AND ((@account_number IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) LIKE @account_number))
					  AND ((@first_name IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) LIKE @first_name))
					  AND ((@last_name IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) LIKE @last_name))
					  AND ((@cms_id IS NULL) OR ([customer_account].cms_id LIKE @cms_id))
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC sp_insert_audit @user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting cards for card search.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [CARD_SEARCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CARD_SEARCH_TRAN]
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







