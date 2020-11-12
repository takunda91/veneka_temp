-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Fetch all issuers who are licensed/unlicensed
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_license_issuers] 
	-- Add the parameters for the stored procedure here
	@issuer_licensed bit = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT issuer_id, issuer_code, issuer_name, license_file, license_key
	FROM [issuer]
	WHERE issuer_status_id = 0 AND
			(@issuer_licensed IS NULL
			OR (@issuer_licensed = 1 AND [issuer].license_key IS NOT NULL)
			OR (@issuer_licensed = 0 AND [issuer].license_key IS NULL))
END