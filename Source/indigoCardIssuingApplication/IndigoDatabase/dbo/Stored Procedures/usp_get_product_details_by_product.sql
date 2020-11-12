
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_details_by_product] 
	-- Add the parameters for the stored procedure here
	@product_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT
		   [product_fee_detail].fee_scheme_id,
		   [product_fee_detail].fee_detail_id,
		   [product_fee_detail].fee_detail_name,
		   [product_fee_detail].fee_waiver_YN,
		   [product_fee_detail].fee_editable_YN
	FROM [issuer_product]
			INNER JOIN [product_fee_detail]
				ON [issuer_product].fee_scheme_id = [product_fee_detail].fee_scheme_id
	WHERE [issuer_product].product_id = @product_id


END