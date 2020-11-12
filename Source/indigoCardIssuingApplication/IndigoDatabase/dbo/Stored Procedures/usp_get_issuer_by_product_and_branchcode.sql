-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_by_product_and_branchcode] 
	-- Add the parameters for the stored procedure here
	@bin_code char(6),
	@branch_code char(3) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DISTINCT [issuer].*
	FROM [issuer]
			INNER JOIN [issuer_product]
				ON [issuer_product].issuer_id = [issuer].issuer_id
			INNER JOIN [branch]
				ON [branch].issuer_id = [issuer].issuer_id
	WHERE [issuer_product].product_bin_code LIKE CONVERT(varchar,@bin_code + '%')
			AND [branch].branch_code = COALESCE(@branch_code, [branch].branch_code)
END