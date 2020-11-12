-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Create card reference number
-- =============================================
CREATE FUNCTION [dbo].[GenCardReferenceNo] 
(
	-- Add the parameters for the function here
	@status_date DATETIMEOFFSET,
	@card_id BIGINT
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result VARCHAR(MAX),
			@product_code varchar(max),
			@issuer_code varchar(max)

	SELECT @issuer_code = [issuer].issuer_code,
			@product_code = [issuer_product].product_code
	FROM [issuer]
		INNER JOIN [branch]
			ON [issuer].issuer_id = [branch].issuer_id
		INNER JOIN [cards]
			ON [branch].branch_id = [cards].branch_id
		INNER JOIN [issuer_product]
			ON [issuer_product].product_id = [cards].product_id
	WHERE [cards].card_id = @card_id


	-- Add the T-SQL statements to compute the return value here
	--SELECT @Result = 'CR' + CONVERT(VARCHAR(8), @status_date, 12) + 
	--						RIGHT('000'+convert(varchar(3), @product_id), 3) +							
	--						CAST(@card_id AS varchar(max))

	SELECT @Result = 'CR' + @issuer_code + @product_code +						
							CAST(@card_id AS varchar(max))


	-- Return the result of the function
	RETURN @Result

END