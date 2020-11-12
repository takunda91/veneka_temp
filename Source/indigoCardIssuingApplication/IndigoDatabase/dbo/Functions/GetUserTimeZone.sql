-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetUserTimeZone]
(
	@user_id bigint
)
RETURNS nvarchar(50)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @time_zone as nvarchar(50)

	-- Add the T-SQL statements to compute the return value here
	SELECT @time_zone = time_zone_utcoffset from dbo.[user] where dbo.[user].user_id=@user_id

	-- Return the result of the function
	RETURN @time_zone

END