CREATE PROCEDURE [dbo].[usp_get_operator_hybrid_request_inprogress] 
	@status_id int,
	@user_id bigint,
	@language_id INT,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	---- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)
	
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

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
		SELECT DISTINCT [hybrid_requests].request_id as 'request_id', 
				[hybrid_requests].[request_reference] AS 'request_reference',	
				[hybrid_requests].product_id, --[cards].sub_product_id, 
				[hybrid_requests].card_priority_id, 
				[hybrid_requests].card_issue_method_id,			   
				[hybrid_request_statuses_language].language_text AS current_card_status,
				[hybrid_request_status_current].comments,						
				CONVERT(Datetime,SWITCHOFFSET([hybrid_request_status_current].status_date,@UserTimezone)) as status_date,	
				[hybrid_request_status_current].hybrid_request_statuses_id as hybrid_request_statuses_id,					
				
				[hybrid_request_status_current].operator_user_id, 
				'' AS operator_username, 
				'' AS product_bin_code,		
				'' AS sub_product_code,	
				[issuer].issuer_id, [issuer].issuer_name, [issuer].issuer_code,
				[branch].branch_id, [branch].branch_name, [branch].branch_code,CAST(0 as BIGINT) as card_id,
				'' AS card_number
		FROM [hybrid_requests]
			INNER JOIN [hybrid_request_status_current]
				ON [hybrid_request_status_current].request_id = [hybrid_requests].request_id						   
			INNER JOIN [hybrid_request_statuses_language]
				ON [hybrid_request_status_current].hybrid_request_statuses_id = [hybrid_request_statuses_language].hybrid_request_statuses_id
					AND [hybrid_request_statuses_language].language_id = @language_id							
			--Filter out cards linked to branches the user doesnt have access to.
			INNER JOIN (SELECT DISTINCT branch_id								
						FROM [user_roles_branch] INNER JOIN [user_roles]
								ON [user_roles_branch].user_role_id = [user_roles].user_role_id		
						WHERE [user_roles_branch].[user_id] = @user_id
								AND [user_roles].user_role_id IN (3,2)--Only want roles that allowed to search cards
						) as X
				ON [hybrid_requests].branch_id = X.branch_id
			INNER JOIN [branch]
				ON [branch].branch_id = [hybrid_requests].branch_id
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [branch].issuer_id
			INNER JOIN [issuer_product]
					ON [issuer_product].product_id = [hybrid_requests].product_id

					WHERE  [branch].branch_status_id = 0					 
				AND [issuer].issuer_status_id = 0	
				AND [hybrid_requests].card_issue_method_id = 1
				AND (([hybrid_request_status_current].hybrid_request_statuses_id = @status_id
						AND [issuer].maker_checker_YN = 1)
					)
				--AND [hybrid_request_status_current].operator_user_id = @user_id				 	  
				
		
	) AS Src )
	SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES
		,*
	FROM PAGE_ROWS
	WHERE ROW_NO BETWEEN @StartRow AND @EndRow
	ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END










