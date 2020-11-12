-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_scheme]
	-- Add the parameters for the stored procedure here
	@fee_scheme_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [product_fee_scheme].*, [issuer].issuer_name, 
				0 as TOTAL_ROWS, 
				CONVERT(bigint, 0) AS ROW_NO,
				0 as TOTAL_PAGES
	FROM [product_fee_scheme]
		INNER JOIN [issuer]	
			ON [product_fee_scheme].issuer_id = [issuer].issuer_id
	WHERE [product_fee_scheme].fee_scheme_id = @fee_scheme_id
END