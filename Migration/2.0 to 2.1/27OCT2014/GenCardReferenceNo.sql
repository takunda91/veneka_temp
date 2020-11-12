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
-- Description:	Create card reference number
-- =============================================
CREATE FUNCTION GenCardReferenceNo 
(
	-- Add the parameters for the function here
	@status_date DATETIME,
	@product_id INT,
	@card_id BIGINT
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result int

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = 'CR' + CONVERT(VARCHAR(8), @status_date, 12) + 
							RIGHT('000'+convert(varchar(3), @product_id), 3) +							
							CAST(@card_id AS varchar(max))


	-- Return the result of the function
	RETURN @Result

END
GO

