-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_cards_in_error] 
	@user_id bigint,
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

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)			
  Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id)

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
		SELECT 
			DISTINCT [cards].card_id
			, CASE 
				WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
				ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
			  END AS 'card_number'
			, cards.card_request_reference AS card_reference_number
			, [cards].product_id, --[cards].sub_product_id, 
			[cards].card_priority_id, [cards].card_issue_method_id				   
			, [branch_card_statuses_language].language_text AS current_card_status
			, [branch_card_status_current].comments					
			, CAST(SWITCHOFFSET([branch_card_status_current].status_date,@UserTimezone) as Datetime) as 'status_date'
			, [branch_card_status_current].branch_card_statuses_id					
			, [issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code
			, [branch].branch_id, [branch].branch_name, [branch].branch_code
			, card_request_reference
			, null as operator_user_id
			, null as operator_username
			, '' as product_bin_code
			, '' AS sub_product_code	
		FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [branch_card_status_current].card_id = [cards].card_id						   
			INNER JOIN [branch_card_statuses_language]
				ON [branch_card_status_current].branch_card_statuses_id = [branch_card_statuses_language].branch_card_statuses_id
					AND [branch_card_statuses_language].language_id = @language_id							
			--Filter out cards linked to branches the user doesnt have access to.
			INNER JOIN (SELECT DISTINCT branch_id								
						FROM [user_roles_branch] INNER JOIN [user_roles]
								ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
						WHERE [user_roles_branch].[user_id] = @user_id
								AND [user_roles].user_role_id = 2--Only want roles that allowed to search cards
						) as X
				ON [cards].branch_id = X.branch_id
			INNER JOIN [branch]
				ON [branch].branch_id = [cards].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
		WHERE  [branch].branch_status_id = 0	 
				AND [issuer].issuer_status_id = 0			  
				AND [branch_card_status_current].branch_card_statuses_id IN (8 , 9) 

	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END