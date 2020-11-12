-- =============================================
-- Author:		Richard
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_currency] 
	-- Add the parameters for the stored procedure here
	@product_id int, 
	@currency_id int = null,
	@active_YN bit = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [currency] LEFT OUTER JOIN [product_currency]
		ON [currency].currency_id = [product_currency].currency_id
		AND [product_currency].product_id = @product_id
	WHERE ((@active_YN IS NULL) OR ([currency].active_YN = @active_YN))
	 AND ((@currency_id IS NULL) OR ([currency].currency_id = @currency_id))

END