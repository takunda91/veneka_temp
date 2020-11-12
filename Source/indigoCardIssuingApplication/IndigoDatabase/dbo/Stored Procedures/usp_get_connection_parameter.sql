-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
Create PROCEDURE [dbo].[usp_get_connection_parameter] 
	-- Add the parameters for the stored procedure here
	@connection_parameter_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT [connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id, timeout_milli, buffer_size, doc_type, name_of_file, 
			file_delete_YN, file_encryption_type_id, duplicate_file_check_YN,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key],domain_name ,is_external_auth, 
			remote_port, 
			CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_username)) as [remote_username], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_password)) as [remote_password],
			CONVERT(VARCHAR(max),DECRYPTBYKEY(nonce)) as nonce
		FROM connection_parameters
		WHERE connection_parameter_id = @connection_parameter_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END
