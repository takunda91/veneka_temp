-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/04/02
-- Description:	Gets ldap settings for
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_ldap_setting]	
	@ldap_setting_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [ldap_setting].domain_name, [ldap_setting].hostname_or_ip, [ldap_setting].[path],
				CONVERT(VARCHAR(max),DECRYPTBYKEY([ldap_setting].username)) as domain_username,
				CONVERT(VARCHAR(max),DECRYPTBYKEY([ldap_setting].[password])) as domain_password,[ldap_setting].auth_type_id,external_inteface_id
		FROM [ldap_setting]
		WHERE [ldap_setting].ldap_setting_id = @ldap_setting_id
		 

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END