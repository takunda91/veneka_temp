-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_interface_conn] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@interface_type_id int,
	@interface_area int,
	@auditUserId bigint,
	@auditWorkstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT TOP 1 [connection_parameters].[connection_parameter_id],[connection_name],[address],[port],[path],[protocol],[auth_type],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([username])) as [username],
			CONVERT(VARCHAR(max),DECRYPTBYKEY([password])) as [password]
		FROM [connection_parameters]
			INNER JOIN [issuer_interface]
				ON [issuer_interface].connection_parameter_id = [connection_parameters].connection_parameter_id
		WHERE [issuer_interface].issuer_id = @issuer_id AND
			  [issuer_interface].interface_type_id = @interface_type_id AND
			  [issuer_interface].interface_area = @interface_area

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END