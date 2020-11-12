-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Retreive a distribution batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_dist_batch] 
	@dist_batch_id bigint,
	@language_id INT,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_DIST_BATCH]
		BEGIN TRY 

		--DECLARE @flow_id int

		--if(NOT EXISTS(SELECT * FROM [dist_batch_statuses_flow] INNER JOIN [dist_batch]
		--			ON [dist_batch_statuses_flow].issuer_id = [dist_batch].issuer_id
		--				AND [dist_batch_statuses_flow].card_issue_method_id = [dist_batch].card_issue_method_id
		--				AND [dist_batch_statuses_flow].dist_batch_type_id = [dist_batch].dist_batch_type_id
		--			WHERE [dist_batch].dist_batch_id = @dist_batch_id))
		--	BEGIN
		--		SET @flow_id = -1
		--	END

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;
		Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
			SELECT 
				distinct CAST(1 as BIGINT) as ROW_NO, 1 AS TOTAL_ROWS, 1 as TOTAL_PAGES
				, [dist_batch].dist_batch_id
				, CAST(SWITCHOFFSET([dist_batch].date_created,@UserTimezone) as DATETIME) as date_created
				, [dist_batch].dist_batch_reference
				, [dist_batch].no_cards
				, [dist_batch_status_current].dist_batch_statuses_id
				, [dist_batch_status_current].status_notes
				, [dist_batch_statuses_language].language_text AS 'dist_batch_status_name'
				, [issuer].issuer_id
				, [issuer].issuer_name
				, [issuer].issuer_code
				--, [issuer].auto_create_dist_batch
				, [card_issue_method_language].language_text AS 'card_issue_method_name'
				, [dist_batch].card_issue_method_id
				, [dist_batch].dist_batch_type_id
				, [product_flow].dist_batch_status_flow_id
				, [product_flow].flow_dist_batch_statuses_id
				, [product_flow].flow_dist_batch_type_id
				, [product_flow].user_role_id
				-- temp fix to remove reject batch status if the branch isnt a card centre
				, CASE WHEN [branch].branch_type_id = 0 AND [dist_batch_status_current].dist_batch_statuses_id = 2 THEN NULL ELSE [product_flow].reject_dist_batch_statuses_id END AS [reject_dist_batch_statuses_id]
				, ISNULL([branch].branch_name, '-') as branch_name
				, ISNULL([branch].branch_code,'') as branch_code
				, [issuer_product].product_name as 'product_name'
				, CONVERT(VARCHAR(max),DECRYPTBYKEY(CREATOR.username)) as [username]
				
			FROM [dist_batch] 
				INNER JOIN [dist_batch_status_current]
					ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id
				INNER JOIN [dist_batch_statuses_language]
					ON [dist_batch_status_current].dist_batch_statuses_id = [dist_batch_statuses_language].dist_batch_statuses_id
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [dist_batch].issuer_id
				INNER JOIN [card_issue_method_language]
					ON [dist_batch].card_issue_method_id = [card_issue_method_language].card_issue_method_id
						AND [card_issue_method_language].language_id = @language_id	
				LEFT OUTER JOIN (SELECT [username], dist_batch_id
							FROM [user]
								INNER JOIN [dist_batch_status]
									ON [dist_batch_status].[user_id] = [user].[user_id]
										AND ([dist_batch_status].dist_batch_statuses_id = 0)
										AND [dist_batch_status].dist_batch_id = @dist_batch_id) AS CREATOR
					ON CREATOR.dist_batch_id = [dist_batch].dist_batch_id

				LEFT OUTER JOIN [branch]
					ON [branch].branch_id = [dist_batch].branch_id	
				INNER JOIN [dist_batch_cards]
					ON [dist_batch].dist_batch_id=[dist_batch_cards].dist_batch_id
				INNER JOIN cards  
					ON [dist_batch_cards].card_id=cards.card_id
				INNER JOIN [issuer_product]
					ON cards.product_id = [issuer_product].product_id
				LEFT OUTER JOIN [dist_batch_statuses_flow] AS [product_flow]
					ON (([dist_batch].dist_batch_type_id = 0 AND 
							[dist_batch].issuer_id IN (SELECT issuer_id FROM [user_roles_issuer] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @audit_user_id) AND
							[product_flow].dist_batch_status_flow_id = [issuer_product].production_dist_batch_status_flow)
						OR ([dist_batch].dist_batch_type_id = 1 AND 
							([dist_batch].branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (1, 2, 4, 5, 11, 12, 13) AND [user_id] = @audit_user_id) 
								OR
							 ([dist_batch].origin_branch_id IN (SELECT branch_id FROM [user_roles_branch] WHERE user_role_id IN (2, 4) AND [user_id] = @audit_user_id) AND
									[dist_batch_status_current].[dist_batch_statuses_id] IN (5, 16))) AND
							
							[product_flow].dist_batch_status_flow_id = [issuer_product].distribution_dist_batch_status_flow))
						AND [product_flow].dist_batch_statuses_id = [dist_batch_status_current].dist_batch_statuses_id

			WHERE [dist_batch].dist_batch_id = @dist_batch_id
					AND [dist_batch_statuses_language].language_id = @language_id	
					
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;	



			COMMIT TRANSACTION [GET_DIST_BATCH]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_DIST_BATCH]
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
