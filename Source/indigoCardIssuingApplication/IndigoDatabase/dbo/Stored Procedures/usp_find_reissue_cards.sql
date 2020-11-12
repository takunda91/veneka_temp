-- =============================================
-- Author:		Richard Brenchley
-- Create date: 28 March 2014
-- Description:	Search for card/s based on input parameters
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_reissue_cards] 
	-- Add the parameters for the stored procedure here
	@user_id bigint,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
	BEGIN TRANSACTION [CARD_REISSUE_SEARCH_TRAN]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

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
			SELECT ROW_NUMBER() OVER(ORDER BY card_number ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS			
					, *
			FROM( 				
				SELECT DISTINCT [cards].card_id, 
					   dbo.MaskString(CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_number)), 6,4) AS card_number,
					   --[load_batch].load_date, 
					   --[load_batch].load_batch_id,
					   --[load_card_statuses].load_card_status, 
					  -- [dist_batch].date_created,					   
					  --[dist_batch].dist_batch_id,
					 --  [dist_card_statuses].dist_card_status_name,
					 --  [branch_card_statuses].branch_card_statuses_name,
					   
					   COALESCE([branch_card_statuses].branch_card_statuses_name, 
								[dist_card_statuses].dist_card_status_name,
								[load_card_statuses].load_card_status) as current_card_status,
					   COALESCE(SWITCHOFFSET([branch_card_status_current].status_date,@UserTimezone) , 
								SWITCHOFFSET([dist_batch_status_current].status_date,@UserTimezone) ,
								SWITCHOFFSET([load_batch_status_current].status_date,@UserTimezone) ) as status_date,
					   [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
					   [branch].branch_id, [branch].branch_name, [branch].branch_code
				FROM [cards]						
					INNER JOIN [load_batch_cards]
						ON [cards].card_id = [load_batch_cards].card_id
					INNER JOIN [load_batch]
						ON [load_batch].load_batch_id = [load_batch_cards].load_batch_id
					INNER JOIN [load_batch_status_current]
						ON [load_batch].load_batch_id = [load_batch_status_current].load_batch_id
					INNER JOIN [load_card_statuses]
						ON [load_card_statuses].load_card_status_id = [load_batch_cards].load_card_status_id
					--Dist batch joins
					LEFT OUTER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id						
					LEFT OUTER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
					LEFT OUTER JOIN [dist_batch_status_current]
						ON [dist_batch].dist_batch_id = [dist_batch_status_current].dist_batch_id				
					LEFT OUTER JOIN [dist_card_statuses]
						ON [dist_card_statuses].dist_card_status_id = [dist_batch_cards].dist_card_status_id
					--branch card joins
					LEFT OUTER JOIN [branch_card_status_current]
						ON [branch_card_status_current].card_id = [cards].card_id						   
					LEFT OUTER JOIN [branch_card_statuses]
						ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses].branch_card_statuses_id						
					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT branch_id								
								FROM [user_roles_branch] INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
								WHERE [user_roles_branch].[user_id] = @user_id
										AND [user_roles].user_role_id IN (3)--Only want roles that allowed to search cards										
								) as X
						ON [cards].branch_id = X.branch_id
					INNER JOIN [branch]
						ON [branch].branch_id = [cards].branch_id
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
					INNER JOIN [issuer_product]
						ON [issuer_product].product_id =[cards].product_id 
				WHERE [branch].branch_status_id = 0	 
					  AND [issuer].issuer_status_id = 0
					  AND (([issuer_product].pin_mailer_printing_YN = 0 AND [branch_card_status_current].branch_card_statuses_id = 4)
							OR
						    ([issuer_product].pin_mailer_printing_YN = 1 AND [branch_card_status_current].branch_card_statuses_id = 5))

			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY card_number ASC

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

			--log the audit record		
			--EXEC usp_insert_audit @user_id, 
			--					 1,
			--					 NULL, 
			--					 @audit_workstation, 
			--					 'Getting cards for card search.', 
			--					 NULL, NULL, NULL, NULL

			COMMIT TRANSACTION [CARD_REISSUE_SEARCH_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CARD_REISSUE_SEARCH_TRAN]
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