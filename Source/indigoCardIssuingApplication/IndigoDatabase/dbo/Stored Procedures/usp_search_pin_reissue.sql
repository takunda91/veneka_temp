CREATE PROCEDURE [dbo].[usp_search_pin_reissue]
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@branch_id int = null,
	@user_role_id int = null,
	@pin_reissue_statuses_id int = null,
	@operator_user_id bigint = null,
	@operator_in_progress bit,
	@authorise_user_id bigint = null,
	@date_from DATETIME = NULL,
	@date_to DATETIME = NULL,
	@language_id int,
	@PageIndex INT = 1,
	@RowsPerPage INT = 20,
	@pin_reissue_type_id INT = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Clean up any old requests that may have expired
	EXEC [dbo].[usp_pin_reissue_expiry_check]

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)
	DECLARE @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

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
					, COUNT(pin_reissue_id) OVER() AS TOTAL_ROWS
					, *
			FROM( 
				SELECT [pin_reissue].authorise_user_id,
					   [pin_reissue].branch_id,
					   [pin_reissue].failed,
					   [pin_reissue].issuer_id,
					   [pin_reissue].notes,
					   [pin_reissue].operator_user_id,
					   [pin_reissue].pan,
					   [pin_reissue].pin_reissue_id,
					   [pin_reissue].primary_index_number,
					   [pin_reissue].product_id,
					   SWITCHOFFSET([pin_reissue].reissue_date, @UserTimezone) AS reissue_date,
					   SWITCHOFFSET([pin_reissue].request_expiry, @UserTimezone) AS request_expiry,
				       CONVERT(VARCHAR(100),DECRYPTBYKEY([pin_reissue].pan)) as 'clear_card_number',	
						[pin_reissue_status_current].pin_reissue_statuses_id,
						SWITCHOFFSET([pin_reissue_status_current].status_date, @UserTimezone) AS status_date, 
						[pin_reissue_status_current].[user_id],
						[pin_reissue_status_current].comments,
						CONVERT(VARCHAR(100),DECRYPTBYKEY([user].username)) as operator_usename,
						CONVERT(VARCHAR(100),DECRYPTBYKEY([authoriser].username)) as authorise_username,
						[pin_reissue_statuses_language].language_text as pin_reissue_statuses_name,
						[user_branch_access].issuer_name,
						[user_branch_access].issuer_code,
						[user_branch_access].branch_name,
						[user_branch_access].branch_code,
						[user_branch_access].authorise_pin_reissue_YN,
						[issuer_product].product_code, 
						[issuer_product].product_name,
						@UserTimezone as user_time_zone,
						CONVERT(VARCHAR(max),DECRYPTBYKEY([pin_reissue].mobile_number)) as mobile_number,pin_reissue_type_id
				FROM [pin_reissue]
					INNER JOIN [pin_reissue_status_current]
						ON [pin_reissue].pin_reissue_id = [pin_reissue_status_current].pin_reissue_id
					INNER JOIN [pin_reissue_statuses_language]
						ON [pin_reissue_status_current].pin_reissue_statuses_id = [pin_reissue_statuses_language].pin_reissue_statuses_id
							AND [pin_reissue_statuses_language].language_id = @language_id
					INNER JOIN [user]
						ON [user].[user_id] = [pin_reissue].operator_user_id
					INNER JOIN [issuer_product]
						ON [issuer_product].product_id = [pin_reissue].product_id
					--Filter out cards linked to branches the user doesnt have access to.
					INNER JOIN (SELECT DISTINCT [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id
												,[issuer].issuer_name, [issuer].issuer_code, authorise_pin_reissue_YN							
								FROM [user_roles_branch] 
									INNER JOIN [user_roles]
										ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
									INNER JOIN [branch]
										ON [user_roles_branch].branch_id = [branch].branch_id	
											AND [branch].branch_status_id = 0
									INNER JOIN [issuer]
										ON [branch].issuer_id = [issuer].issuer_id
											AND [issuer].issuer_status_id = 0
											AND [issuer].enable_instant_pin_YN = 1
								WHERE [user_roles_branch].[user_id] = @audit_user_id		
										AND [user_roles_branch].issuer_id = COALESCE(@issuer_id, [user_roles_branch].issuer_id)								
										AND [user_roles_branch].branch_id = COALESCE(@branch_id, [user_roles_branch].branch_id)										
										AND [user_roles_branch].user_role_id = COALESCE(@user_role_id, [user_roles_branch].user_role_id)
										AND [user_roles].user_role_id IN (1,2,7)--Only want roles that allowed to search cards
								) as [user_branch_access]
						ON [pin_reissue].branch_id = [user_branch_access].branch_id
						LEFT OUTER JOIN [user] as [authoriser]
							ON [authoriser].[user_id] = [pin_reissue].authorise_user_id
				WHERE 
					((@operator_in_progress = 1 AND (([user_branch_access].authorise_pin_reissue_YN = 0 AND  [pin_reissue_status_current].pin_reissue_statuses_id = 0)
												OR ([user_branch_access].authorise_pin_reissue_YN = 1 AND  [pin_reissue_status_current].pin_reissue_statuses_id = 1))) OR
					 (@operator_in_progress = 0 AND [pin_reissue_status_current].pin_reissue_statuses_id = COALESCE(@pin_reissue_statuses_id, [pin_reissue_status_current].pin_reissue_statuses_id)))
					 
					AND [pin_reissue].operator_user_id = COALESCE(@operator_user_id, [pin_reissue].operator_user_id)
					--AND [pin_reissue].authorise_user_id = COALESCE(@authorise_user_id, [pin_reissue].authorise_user_id)
					AND [pin_reissue_status_current].status_date BETWEEN COALESCE(@date_from, [pin_reissue_status_current].status_date) AND COALESCE(@date_to, [pin_reissue_status_current].status_date)
					AND	[pin_reissue].pin_reissue_type_id=@pin_reissue_type_id
			) AS Src )
			SELECT CAST(CEILING(TOTAL_ROWS/ CAST(@RowsPerPage AS DECIMAL(20,2))) AS INT) AS TOTAL_PAGES, ROW_NO, TOTAL_ROWS
				, CASE 
					WHEN @mask_screen = 1 THEN [dbo].[MaskString](clear_card_number,6,4) 
					ELSE clear_card_number
				  END AS 'card_number'
				, authorise_user_id, branch_id, failed, issuer_id, notes, operator_user_id, pan, pin_reissue_id, primary_index_number
				, product_id, reissue_date, request_expiry,	pin_reissue_statuses_id, status_date, [user_id], comments, operator_usename
				, authorise_username, pin_reissue_statuses_name, issuer_name, issuer_code, branch_name,	branch_code, authorise_pin_reissue_YN
				, product_code, product_name, user_time_zone
			FROM PAGE_ROWS
			WHERE ROW_NO BETWEEN @StartRow AND @EndRow
			--ORDER BY status_date DESC

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
GO

