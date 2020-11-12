-- =============================================
-- Author:		sandhya konduru
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getproductcode]
	-- Add the parameters for the stored procedure here
	@issuerid int,
	@bincode varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	select top 1 * from issuer_product
	where product_bin_code=@bincode and issuer_id=@issuerid

	--select top 1 product_code,product_name from issuer_product
	--where product_bin_code=@bincode and issuer_id=@issuerid

END