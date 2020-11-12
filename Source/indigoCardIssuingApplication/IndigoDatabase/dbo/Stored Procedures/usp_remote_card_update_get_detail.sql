CREATE PROCEDURE [dbo].[usp_remote_card_update_get_detail]
	@card_id bigint,
	@language_id int = 0,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	DECLARE	@access_table TABLE (card_id bigint, branch_id int, branch_code varchar(250), branch_name varchar(250), issuer_id int, issuer_code varchar(250), issuer_name varchar(250))

	--Get cards the user may access based on their role
	INSERT INTO @access_table (card_id, branch_id, branch_code, branch_name, issuer_id, issuer_name, issuer_code)
	SELECT DISTINCT card_id, [user_roles_branch].branch_id, [branch].branch_code, [branch].branch_name, [branch].issuer_id, [issuer].issuer_code, [issuer].issuer_name							
	FROM [user_roles_branch] 
		INNER JOIN [user_roles]
			ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
		INNER JOIN [branch]
			ON [user_roles_branch].branch_id = [branch].branch_id	
				AND [branch].branch_status_id = 0
		INNER JOIN [issuer]
			ON [branch].issuer_id = [issuer].issuer_id
				AND [issuer].issuer_status_id = 0
		INNER JOIN [cards]
			ON [cards].branch_id = [branch].branch_id
		INNER JOIN [issuer_product]
			ON [issuer_product].product_id = [cards].product_id
				AND [issuer].issuer_id = [issuer_product].issuer_id
	WHERE [user_roles_branch].[user_id] = @audit_user_id		
		AND [cards].card_id = @card_id		
		AND [user_roles].user_role_id IN (1,4,5,12)--Only want roles that allowed to search cards


	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT 
		[remote_update_current].[remote_update_statuses_id], 
		[remote_update_statuses_language].language_text as 'remote_update_statuses_name',
		[remote_update_current].[card_id], 
		 CAST(SWITCHOFFSET([status_date],@UserTimezone) as datetime) as 'status_date', 
		[comments], 
		[remote_component], 
		[user_id], 
		 CAST(SWITCHOFFSET([remote_updated_time],@UserTimezone) as datetime) as 'remote_updated_time', 
		[card_request_reference],
		[branch_code],
		[branch_name],
		[issuer_id],
		[issuer_code],
		[issuer_name],
		CASE 
			WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
			ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
		END as card_number
	FROM [dbo].[remote_update_current] INNER JOIN [cards] 
			ON [cards].card_id = [remote_update_current].card_id
		INNER JOIN [dbo].[remote_update_statuses_language]
			ON [remote_update_statuses_language].[remote_update_statuses_id] = [remote_update_current].remote_update_statuses_id
				AND [remote_update_statuses_language].language_id = @language_id
		INNER JOIN @access_table [access]
			ON [cards].card_id = [access].card_id
	WHERE [remote_update_current].card_id = @card_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
