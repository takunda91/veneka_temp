-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_charges] 
	-- Add the parameters for the stored procedure here
	@fee_detail_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [currency].*
		, ISNULL([product_fee_charge].fee_charge, 0) as fee_charge
		, ISNULL([product_fee_charge].vat, 0) as vat,[product_fee_charge].cbs_account_type,product_fee_charge.card_issue_reason_id
	FROM [currency]	
			INNER JOIN [product_fee_charge]
				ON [currency].currency_id = [product_fee_charge].currency_id
					AND [product_fee_charge].fee_detail_id = @fee_detail_id
END