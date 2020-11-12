-- =============================================
-- Author:		Lebo Tladi
-- Create date:
-- Description:	Returns decrypted instant authorisation pin
-- =============================================
 CREATE PROCEDURE [dbo].[usp_get_authpin_by_user_id] 
	@username varchar(200),
	@branch_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT [user].[user_id],
					CONVERT(VARCHAR(max),DECRYPTBYKEY([user].instant_authorisation_pin)) as 'instant_authorisation_pin'
			FROM [user_roles_branch]
				INNER JOIN [user]
				ON [user_roles_branch].[user_id] = [user].[user_id]
					AND [user_roles_branch].user_role_id = 2
					AND [user_roles_branch].branch_id = @branch_id
					AND [user].user_status_id = 0
				INNER JOIN [branch]
					ON [branch].branch_id = [user_roles_branch].branch_id
						AND [branch].branch_status_id = 0
				INNER JOIN [issuer]
					ON [issuer].issuer_id = [branch].issuer_id
						AND [issuer].issuer_status_id = 0
			WHERE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([user].username)) = @username

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END