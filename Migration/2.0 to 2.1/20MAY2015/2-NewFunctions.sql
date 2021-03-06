USE [indigo_database_main_dev]
GO
/****** Object:  UserDefinedFunction [dbo].[MaskReportPAN]    Script Date: 2015-05-20 10:15:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchey
-- Create date: 
-- Description:	Determins if the users should have PAN masked for screen or not
-- =============================================
CREATE FUNCTION [dbo].[MaskReportPAN] 
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
			SELECT [user_group].mask_report_pan
			FROM [user_group]
				INNER JOIN [users_to_users_groups]
					ON [user_group].user_group_id = [users_to_users_groups].user_group_id
			WHERE [users_to_users_groups].[user_id] = @user_id
				AND [user_group].mask_report_pan = 1				
		)THEN 1 
		ELSE 0 END

	-- Return the result of the function
	RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[MaskScreenPAN]    Script Date: 2015-05-20 10:15:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
