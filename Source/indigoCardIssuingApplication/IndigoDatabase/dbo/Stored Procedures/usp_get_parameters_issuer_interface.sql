-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_parameters_issuer_interface] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@interface_type_id int,
	@interface_area int,
	@interface_guid char(36) = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

    SELECT issuer_interface.interface_guid
		, connection_parameters.[connection_parameter_id]
		, connection_parameters.connection_parameter_type_id
		, [connection_name]
		, [address]
		, [port]
		, [path]
		, [protocol]
		, [auth_type]
		, [header_length]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY(nonce)) as nonce
		, connection_parameter_type_id
		, timeout_milli
		, buffer_size
		, doc_type
		, name_of_file
		, file_delete_YN
		, file_encryption_type_id
		, duplicate_file_check_YN
		, connection_parameters.remote_port
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key]
		, CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_username)) as [remote_username] 
		, CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_password)) as [remote_password]
	FROM connection_parameters
		INNER JOIN issuer_interface
			ON connection_parameters.connection_parameter_id = issuer_interface.connection_parameter_id
	WHERE issuer_interface.issuer_id = COALESCE(@issuer_id, issuer_interface.issuer_id)
			AND issuer_interface.interface_type_id = @interface_type_id
			AND issuer_interface.interface_area = @interface_area
			AND issuer_interface.interface_guid = COALESCE(@interface_guid, issuer_interface.interface_guid)

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END