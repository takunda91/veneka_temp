USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_operator_cards_inprogress]    Script Date: 2014/08/19 12:24:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_operator_cards_inprogress] 
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
				CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,					   
				[branch_card_statuses_language].language_text AS current_card_status,						
				[branch_card_status_current].status_date,						
				[issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				[branch].branch_id, [branch].branch_name, [branch].branch_code
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
								AND [user_roles].user_role_id IN (3)--Only want roles that allowed to search cards
						) as X
				ON [cards].branch_id = X.branch_id
			INNER JOIN [branch]
				ON [branch].branch_id = [cards].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
		WHERE  [branch].branch_status_id = 0					 
				AND [issuer].issuer_status_id = 0	
				AND [cards].card_issue_method_id = 1
				AND [branch_card_status_current].operator_user_id = @user_id			  
				AND ([branch_card_status_current].branch_card_statuses_id = 3
					OR ([issuer].pin_mailer_printing_YN = 0 AND 
						[branch_card_status_current].branch_card_statuses_id = 4))
		UNION
		SELECT DISTINCT [cards].card_id, 
				CONVERT(VARCHAR,DECRYPTBYKEY([cards].card_number)) AS card_number,					   
				[branch_card_statuses_language].language_text AS current_card_status,						
				[branch_card_status_current].status_date,						
				[issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				[branch].branch_id, [branch].branch_name, [branch].branch_code
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
								AND [user_roles].user_role_id = 7--Only want roles that allowed to search cards
						) as X
				ON [cards].branch_id = X.branch_id
			INNER JOIN [branch]
				ON [branch].branch_id = [cards].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
		WHERE  [branch].branch_status_id = 0	 
				AND [issuer].issuer_status_id = 0	
				AND [cards].card_issue_method_id = 1		  
				AND ([branch_card_status_current].branch_card_statuses_id = 5
					OR ([issuer].pin_mailer_printing_YN = 1 AND 
						[branch_card_status_current].branch_card_statuses_id = 6))
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END


