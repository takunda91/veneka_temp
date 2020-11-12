-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_currency_iso_numeric_code] 
	-- Add the parameters for the stored procedure here
	@currency_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP 1 iso_4217_numeric_code
	FROM [currency]
	WHERE currency_id = @currency_id

END