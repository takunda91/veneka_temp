-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_pin_mailer_reprint]
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_id int,
	@user_role_id int,
	@pin_mailer_reprint_status_id int,
	@language_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	
	
	DECLARE @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
    DECLARE @StartRow INT, @EndRow INT;			

			SET @StartRow = ((@PageIndex - 1) * @RowsPerPage) + 1;
			SET @EndRow = @StartRow + @RowsPerPage - 1;

			--append#1
			WITH PAGE_ROWS
			AS
			(
			SELECT ROW_NUMBER() OVER(ORDER BY status_date DESC, current_reprint_status ASC) AS ROW_NO
					, COUNT(*) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT [cards].card_id,  
					   CASE 
							WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
							ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
						END AS 'card_number',
					   [cards].product_id, 
					   --[cards].sub_product_id, 
					   [cards].card_issue_method_id, 
					   [cards].card_priority_id, 
					   [cards].card_request_reference,
					   [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id, 
					   [pin_mailer_reprint_statuses_language].language_text as 'current_reprint_status',
					  CONVERT(DATETIME2,SWITCHOFFSET([pin_mailer_reprint_status_current].status_date, @UserTimezone) ) AS status_date, 
					  
					   [issuer_product].product_bin_code,
					   [user_branch_access].issuer_id, issuer_name, branch_name, issuer_code, branch_code, [cards].branch_id,
					   [pin_mailer_reprint_status_current].comments
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
					INNER JOIN [pin_mailer_reprint_status_current]
						ON [pin_mailer_reprint_status_current].card_id = [cards].card_id
					INNER JOIN [pin_mailer_reprint_statuses_language]
						ON [pin_mailer_reprint_statuses_language].pin_mailer_reprint_status_id = [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id
							AND [pin_mailer_reprint_statuses_language].language_id = @language_id
				WHERE [pin_mailer_reprint_status_current].pin_mailer_reprint_status_id = @pin_mailer_reprint_status_id
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
				,*
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			ORDER BY status_date DESC, current_reprint_status ASC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
GO

