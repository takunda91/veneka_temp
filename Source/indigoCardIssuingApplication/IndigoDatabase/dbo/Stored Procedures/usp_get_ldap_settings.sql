-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_ldap_settings] 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [ldap_setting_id],[ldap_setting_name], [hostname_or_ip], [domain_name],[path],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password],[ldap_setting].auth_type_id,external_inteface_id
		FROM ldap_setting

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END