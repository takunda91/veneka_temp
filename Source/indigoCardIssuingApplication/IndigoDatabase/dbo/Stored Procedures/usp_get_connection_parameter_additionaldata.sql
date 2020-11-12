-- =============================================
-- Author:		
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_connection_parameter_additionaldata] 
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

		SELECT *
		FROM [connection_parameters_additionaldata]
		WHERE connection_parameter_id = @connection_parameter_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;

END