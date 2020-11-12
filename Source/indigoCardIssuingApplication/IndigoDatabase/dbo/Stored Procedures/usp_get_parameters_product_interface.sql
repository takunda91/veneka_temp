-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_parameters_product_interface] 
	-- Add the parameters for the stored procedure here
	@product_id int = null,
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

    SELECT  product_interface.interface_guid
			, connection_parameters.[connection_parameter_id]
			, connection_parameters.connection_parameter_type_id
			, [connection_name],[address],[port],[path],[protocol],
			[auth_type],[header_length],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([identification])) as identification,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password], 
			connection_parameter_type_id,
			timeout_milli, buffer_size, doc_type, name_of_file, file_delete_YN, file_encryption_type_id, duplicate_file_check_YN, connection_parameters.remote_port,
			CONVERT(VARCHAR(max),DECRYPTBYKEY([private_key])) as [private_key],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([public_key])) as [public_key],
			CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_username)) as [remote_username], 
			CONVERT(VARCHAR(max),DECRYPTBYKEY(remote_password)) as [remote_password],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([nonce])) as [nonce] 
	FROM connection_parameters
		INNER JOIN product_interface
			ON connection_parameters.connection_parameter_id = product_interface.connection_parameter_id
	WHERE product_interface.product_id = COALESCE(@product_id, product_interface.product_id)
			AND product_interface.interface_type_id = @interface_type_id
			AND product_interface.interface_area = @interface_area
			AND product_interface.interface_guid = COALESCE(@interface_guid, product_interface.interface_guid)

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END