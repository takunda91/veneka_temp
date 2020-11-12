-- =============================================
-- Author:		Richard Brenchley
-- Create date: 17 March 2014
-- Description:	Get a branch by its Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branch_by_id] 
	-- Add the parameters for the stored procedure here
	@branch_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  *
	FROM branch
	WHERE branch_id = @branch_id;
END