-- =============================================
-- Author:		Richard Brenchey
-- Create date: 
-- Description:	Determins if the users should have PAN masked for screen or not
-- =============================================
CREATE FUNCTION [dbo].[MaskScreenPAN] 
(
	-- Add the parameters for the function here
	@user_id bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result bit

	--Check if the user belongs to any user groups that have mask screen pan set to true
	SELECT @Result =
		CASE WHEN EXISTS (
			SELECT [user_group].mask_screen_pan
			FROM [user_group]
				INNER JOIN [users_to_users_groups]
					ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			WHERE [users_to_users_groups].[user_id] = @user_id
				AND [user_group].mask_screen_pan = 1				
		)THEN 1 
		ELSE 0 END

	-- Return the result of the function
	RETURN @Result

END