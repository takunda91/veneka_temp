USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_search_cards]    Script Date: 2015/04/30 03:27:12 PM ******/
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
	@dist_batch_id bigint = NULL,
	@pin_batch_id bigint=NULL,

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
			DECLARE @objid int,
					@card_index varbinary(max)
			SET @objid = object_id('cards')

			IF (@card_last_four_digits IS NOT NULL)
				SET @card_index =  [dbo].[MAC] (@card_last_four_digits, @objid)
			
			DECLARE @StartRow INT, @EndRow INT;			
			
			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--Table variable for branches the user has access to, filtered out by branch and issuer
			DECLARE @branches_user TABLE (branch_id int, branch_code varchar(100), branch_name varchar(100),
					issuer_id int, issuer_name varchar(100), issuer_code varchar(100), card_ref_preference bit)

			INSERT INTO @branches_user (branch_id, branch_code, branch_name, issuer_id, issuer_name, issuer_code, card_ref_preference)
			SELECT DISTINCT [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id
														,[issuer].issuer_name, [issuer].issuer_code, card_ref_preference							
										FROM [user_roles_branch] 
											INNER JOIN [user_roles]
												ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
											INNER JOIN [branch]
												ON [user_roles_branch].branch_id = [branch].branch_id	
													AND [branch].branch_status_id = 0
											INNER JOIN [issuer]
												ON [branch].issuer_id = [issuer].issuer_id
													AND [issuer].issuer_status_id = 0
										WHERE [user_roles_branch].[user_id] = @user_id		
												AND [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)								
												AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)										
												AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
												AND [user_roles].user_role_id IN (1,2,3,4,5,7,11,12,13,14,15)--Only want roles that allowed to search cards

			IF OBJECT_ID('tempdb..#Cards') IS NOT NULL
				DROP TABLE #Cards

			--Temp table to store all the cards that match the filters
			CREATE TABLE #Cards(card_id BIGINT PRIMARY KEY
							   ,product_id int, sub_product_id int null
							   ,card_priority_id int, card_issue_method_id int
							   ,branch_id int, branch_code varchar(100), branch_name varchar(100)
							   ,issuer_id int, issuer_code varchar(100), issuer_name varchar(100)					   
							   ,card_number varchar(100), card_request_reference varchar(100))

			INSERT INTO #Cards (card_id, product_id, sub_product_id, card_priority_id, card_issue_method_id, card_number, 
								card_request_reference, branch_id, branch_code, branch_name, issuer_id, issuer_code, issuer_name)
			SELECT 
				card_id
				, product_id
				, sub_product_id
				, card_priority_id
				, card_issue_method_id
				, CASE WHEN [issuer].card_ref_preference = 1 
					THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
					ELSE CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) 
					END 
				AS 'card_number'
				, [cards].card_request_reference
				, [cards].branch_id
				, branch_code
				, branch_name
				, [branches_for_user].issuer_id
				, [branches_for_user].issuer_code
				, [branches_for_user].issuer_name
			FROM [cards]
					INNER JOIN @branches_user as [branches_for_user]
						ON [cards].branch_id = [branches_for_user].branch_id
					INNER JOIN [issuer] ON issuer.issuer_id = [branches_for_user].issuer_id
			WHERE 
							((@card_number IS NULL) OR (DECRYPTBYKEY([cards].card_number) LIKE @card_number))
							AND ((@card_issue_method_id IS NULL) OR ([cards].card_issue_method_id = @card_issue_method_id))
							AND ((@card_last_four_digits IS NULL) OR ([cards].[card_index] = @card_index))
							AND ((@product_id IS NULL) OR ([cards].product_id = @product_id))
							AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
							AND ((@priority_id IS NULL) OR ([cards].card_priority_id  = @priority_id))
							AND ((@card_refnumber IS NULL) OR ([cards].card_request_reference = @card_refnumber));


			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS			
					, *
			FROM( 
					SELECT DISTINCT TOP 1000
					    [cards_temp].card_id
					   ,[cards_temp].product_id
					   ,[cards_temp].sub_product_id
					   ,[cards_temp].card_priority_id
					   ,[cards_temp].card_issue_method_id
					   ,[cards_temp].branch_id
					   ,[cards_temp].card_number
					   ,[cards_temp].issuer_id
					   ,issuer_code, issuer_name			  
					   ,branch_code, branch_name
					   ,COALESCE([branch_card_status_current].comments,
								 [dist_batch_status_card_current].status_notes,
								 [load_batch_status_card_current].status_notes,
								 '') as comments
					   ,COALESCE([branch_card_statuses_language].language_text, 
							     [dist_card_statuses_language].language_text,
							     [load_card_statuses_language].language_text) as current_card_status
					   ,COALESCE([branch_card_status_current].status_date, 
								 [dist_batch_status_card_current].status_date,
								 [load_batch_status_card_current].status_date) as status_date
						,ISNULL([branch_card_status_current].branch_card_statuses_id, 0) as branch_card_statuses_id
						,[cards_temp].card_request_reference
						, null as operator_user_id
						, null as operator_username
						, '' as product_bin_code
				FROM #Cards as [cards_temp]
					--Load Batch Joins
					LEFT OUTER JOIN [load_batch_status_card_current]
						ON [cards_temp].card_id = [load_batch_status_card_current].card_id
						 
					LEFT OUTER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_status_card_current].load_batch_id
					LEFT OUTER JOIN [load_card_statuses_language]
						ON [load_card_statuses_language].load_card_status_id = [load_batch_status_card_current].load_card_status_id
							AND [load_card_statuses_language].language_id = 0

					--Dist Batch Joins
					LEFT OUTER JOIN  [dist_batch_status_card_current]
						ON [cards_temp].card_id = [dist_batch_status_card_current].card_id
							--AND [dist_batch_status_card_current].dist_card_status_id = 0
					LEFT OUTER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_status_card_current].dist_batch_id
					LEFT OUTER JOIN [dist_card_statuses_language]
						ON [dist_card_statuses_language].dist_card_status_id = [dist_batch_status_card_current].dist_card_status_id
							AND [dist_card_statuses_language].language_id = 0

					--branch card joins
					LEFT OUTER JOIN [branch_card_status_current]
						ON [cards_temp].card_id = [branch_card_status_current].card_id						   
					LEFT OUTER JOIN [branch_card_statuses_language]
						ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
							AND [branch_card_statuses_language].language_id = 0

					--Link to customer
					LEFT JOIN [customer_account]
						ON [cards_temp].card_id = [customer_account].card_id

					LEFT Join pin_batch_cards
						ON [cards_temp].card_id =[pin_batch_cards].card_id

				WHERE --Customer Search
					 ((@account_number IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) LIKE @account_number))
					AND ((@first_name IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) LIKE @first_name))
					AND ((@last_name IS NULL) OR (CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) LIKE @last_name))
					AND ((@cms_id IS NULL) OR ([customer_account].cms_id LIKE @cms_id))
					--Other
					AND ((@dist_batch_id IS NULL) OR ([dist_batch].dist_batch_id = @dist_batch_id))
					AND ((@load_card_status_id IS NULL) OR ([load_batch_status_card_current].load_card_status_id = @load_card_status_id))
					AND ((@dist_card_status_id IS NULL) OR ([dist_batch_status_card_current].dist_card_status_id = @dist_card_status_id))
					AND ((@branch_card_statuses_id IS NULL) OR ([branch_card_status_current].branch_card_statuses_id = @branch_card_statuses_id))

					AND ((@batch_reference IS NULL) OR ([load_batch].load_batch_reference LIKE @batch_reference	OR [dist_batch].dist_batch_reference LIKE @batch_reference))	
					  	

					AND (([dist_batch].date_created BETWEEN COALESCE(@date_from, [dist_batch].date_created) AND COALESCE(@date_to, [dist_batch].date_created))
						OR
						([load_batch].load_date BETWEEN COALESCE(@date_from, [load_batch].load_date) AND COALESCE(@date_to, [load_batch].load_date))
						OR
						([branch_card_status_current].status_date BETWEEN COALESCE(@date_from, [branch_card_status_current].status_date) AND COALESCE(@date_to, [branch_card_status_current].status_date)))

						AND  ((@pin_batch_id IS NULL) OR ([pin_batch_cards].pin_batch_id = @pin_batch_id))

			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			DROP TABLE #Cards
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








