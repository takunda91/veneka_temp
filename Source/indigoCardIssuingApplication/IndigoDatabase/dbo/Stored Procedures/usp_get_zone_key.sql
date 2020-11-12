-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_zone_key] 
	-- Add the parameters for the stored procedure here
	@issuer_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY key_injection_keys
	DECRYPTION BY CERTIFICATE cert_ZoneMasterKeys;

    SELECT 	CONVERT(VARCHAR(MAX),DECRYPTBYKEY([zone_keys].zone)) as zone, 
			CONVERT(VARCHAR(MAX),DECRYPTBYKEY([zone_keys].final)) as final
	FROM [zone_keys]
			INNER JOIN [issuer]
				ON [issuer].issuer_id = [zone_keys].issuer_id
					AND [issuer].issuer_status_id = 0
	WHERE [zone_keys].issuer_id = @issuer_id

	CLOSE SYMMETRIC KEY key_injection_keys
END