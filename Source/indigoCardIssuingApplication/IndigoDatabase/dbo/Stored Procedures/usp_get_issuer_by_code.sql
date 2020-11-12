-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_by_code] 
	-- Add the parameters for the stored procedure here
	@issuer_code varchar(10),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 *
	FROM [issuer]
	WHERE issuer_code = @issuer_code
END