-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Check to see if any cards have been added to the card table with no subproducts
-- =============================================
CREATE FUNCTION CheckCardsForProduct 
(
	-- Add the parameters for the function here
	@product_id int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result int

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = COUNT(*)
	FROM [cards]
	WHERE product_id = @product_id
			AND sub_product_id IS NULL

	-- Return the result of the function
	RETURN @Result

END
GO

