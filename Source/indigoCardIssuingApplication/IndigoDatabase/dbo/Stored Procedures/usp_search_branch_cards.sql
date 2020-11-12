CREATE PROCEDURE [dbo].[usp_search_branch_cards]
	@issuer_id int = NULL, 
	@branch_id int = NULL,
	@user_role_id int = NULL,
	@product_id int = NULL,
	--@sub_product_id int = NULL,
	@priority_id int = NULL,
	@card_issue_method_id int = null,
	@card_number varchar(20) = NULL,
	@branch_card_statuses_id int = NULL,
	@operator_user_id bigint = NULL,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [BRANCH_CARD_SEARCH_TRAN]
		BEGIN TRY 

			DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)
			Declare @UserTimezone  varchar(50)=[dbo].[GetUserTimeZone](@audit_user_id) 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--THIS IS FOR QUICKER CARD LOOKUP
			DECLARE @objid int,
					@card_index varbinary(max)
			SET @objid = object_id('cards')

			IF LEN(@card_number) = 4
				BEGIN
					SET @card_index = [dbo].[MAC] (@card_number, @objid)
					SET @card_number = NULL
				END

			DECLARE @StartRow INT, @EndRow INT;			
			
			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;


			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY clear_card_number ASC, status_date DESC) AS ROW_NO
					, COUNT(card_id) OVER() AS TOTAL_ROWS			
					, *
			FROM( 

				SELECT [cards].card_id,  
					   CONVERT(VARCHAR(100),DECRYPTBYKEY([cards].card_number)) as 'clear_card_number',	
					   [cards].[card_request_reference],
					   [cards].product_id,  
					   [cards].card_issue_method_id, 
					   [cards].card_priority_id, 
					   [branch_card_status_current].branch_card_statuses_id, 
					   [branch_card_statuses_language].language_text as current_card_status,
					   [branch_card_status_current].operator_user_id, 
					   cast(SWITCHOFFSET([branch_card_status_current].status_date,@UserTimezone) as datetime) as  status_date, 
					   CONVERT(VARCHAR(100),DECRYPTBYKEY([user].username)) AS operator_username, 
					   [issuer_product].product_bin_code,
					   [issuer_product].sub_product_code,
					   [user_branch_access].issuer_id, issuer_name, branch_name, issuer_code, branch_code, [cards].branch_id,
					   [branch_card_status_current].comments
				FROM [cards]
					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id
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
								WHERE [user_roles_branch].[user_id] = @audit_user_id		
										AND [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)								
										AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)										
										AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
										AND [user_roles].user_role_id IN (1,2,3,4,5,7)--Only want roles that allowed to search cards
								) as [user_branch_access]
						ON [cards].branch_id = [user_branch_access].branch_id
					INNER JOIN [issuer_product]
						ON [cards].product_id = [issuer_product].product_id
					INNER JOIN [branch_card_status_current]
						ON [cards].card_id = [branch_card_status_current].card_id
					INNER JOIN [branch_card_statuses_language]
							ON [branch_card_statuses_language].branch_card_statuses_id = [branch_card_status_current].branch_card_statuses_id
								AND [branch_card_statuses_language].language_id = @language_id
					--INNER JOIN [branch]
					--	ON [branch].branch_id = [cards].branch_id
					LEFT OUTER JOIN [user]
						ON [branch_card_status_current].operator_user_id = [user].[user_id]
				WHERE [cards].product_id = COALESCE(@product_id, [cards].product_id) AND
					  [cards].card_priority_id = COALESCE(@priority_id, [cards].card_priority_id) AND
					  [cards].card_issue_method_id = COALESCE(@card_issue_method_id, [cards].card_issue_method_id) AND
					  ((@card_number IS NULL) OR (DECRYPTBYKEY([cards].card_number) LIKE @card_number))	AND
					  [branch_card_status_current].branch_card_statuses_id = COALESCE(@branch_card_statuses_id, [branch_card_status_current].branch_card_statuses_id) AND
					  ISNULL([branch_card_status_current].operator_user_id, -999) = COALESCE(@operator_user_id, [branch_card_status_current].operator_user_id, -999) 				  
					  AND ((@card_index IS NULL) OR ([cards].[card_index] = @card_index))
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES, ROW_NO, TOTAL_ROWS
				, CASE 
					WHEN @mask_screen = 1 THEN [dbo].[MaskString](clear_card_number,6,4) 
					ELSE clear_card_number
				  END AS 'card_number'
				, card_id, [card_request_reference], product_id, card_issue_method_id, card_priority_id, branch_card_statuses_id, current_card_status
			    , operator_user_id, status_date, operator_username, product_bin_code, sub_product_code, issuer_id, issuer_name, branch_name, issuer_code, branch_code
				, branch_id, comments
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY  clear_card_number ASC,status_date DESC
			
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC usp_insert_audit @audit_user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting cards for branch card search.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [BRANCH_CARD_SEARCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [BRANCH_CARD_SEARCH_TRAN]
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