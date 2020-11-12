-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 March 2014
-- Description:	Find issuers based on status
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_issuer_by_status] 
	-- Add the parameters for the stored procedure here
	@issuer_status_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT * 
	FROM issuer 
	WHERE issuer_status_id = @issuer_status_id

END