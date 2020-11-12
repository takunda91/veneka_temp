-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_tmk_by_terminal] 
	-- Add the parameters for the stored procedure here
	@device_id varchar(250), 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	IF(@audit_user_id != -2)
		BEGIN
			SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) as 'masterkey'
					,[user_branch_access].issuer_id
					,[user_branch_access].branch_id
					,[masterkeys].[masterkey_name]
					,[user_branch_access].issuer_name
					,[masterkeys].masterkey_id
			FROM [masterkeys]
					INNER JOIN [terminals]
						ON [masterkeys].masterkey_id = [terminals].terminal_masterkey_id
					INNER JOIN 
						(SELECT DISTINCT [user_roles_branch].branch_id, [issuer].issuer_id, [issuer].issuer_name						
							FROM [user_roles_branch] 
								INNER JOIN [user_roles]
									ON [user_roles_branch].user_role_id = [user_roles].user_role_id											
								INNER JOIN [branch]
									ON [user_roles_branch].branch_id = [branch].branch_id	
										AND [branch].branch_status_id = 0
								INNER JOIN [issuer]
									ON [branch].issuer_id = [issuer].issuer_id
										AND [issuer].issuer_status_id = 0
							WHERE [user_roles_branch].user_role_id = 7
								AND [user_roles_branch].[user_id] = @audit_user_id	
							) AS [user_branch_access]
						ON [terminals].branch_id = [user_branch_access].branch_id
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([terminals].device_id)) = @device_id
		END
	ELSE
		--Special case for SYSTEM user
		BEGIN
			SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY([masterkeys].masterkey)) as 'masterkey'
					,[branch].issuer_id
					,[branch].branch_id
					,[masterkeys].[masterkey_name]
					,[issuer].issuer_name
					,[masterkeys].masterkey_id
			FROM [masterkeys]
					INNER JOIN [terminals]
						ON [masterkeys].masterkey_id = [terminals].terminal_masterkey_id
					INNER JOIN [branch] 
						ON [branch].branch_id = [terminals].branch_id
							AND [branch].branch_status_id = 0
					INNER JOIN [issuer]
						ON [issuer].issuer_id = [branch].issuer_id
							AND [issuer].issuer_status_id = 0		
			WHERE CONVERT(VARCHAR(max),DECRYPTBYKEY([terminals].device_id)) = @device_id
		END

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END