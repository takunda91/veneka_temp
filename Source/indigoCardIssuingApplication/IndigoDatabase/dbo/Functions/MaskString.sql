
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Masks the input string, left pad are the amount of characters from the left
-- you dont want masked, right pad are the amount of character from the right you dont want masked.
-- =============================================
CREATE FUNCTION [dbo].[MaskString]
(
	@InputStr varchar(100),
	@LeftPad int,
	@rightPad int
)
RETURNS varchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result varchar(100)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = LEFT(@InputStr, @LeftPad) +
					 '******' +
					 RIGHT(@InputStr, @rightPad)

	-- Return the result of the function
	RETURN @Result

END