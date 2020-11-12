-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_by_branch] 
	@branch_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM [issuer]
		INNER JOIN [branch]
			ON [issuer].issuer_id = [branch].issuer_id
	WHERE [branch].branch_id = @branch_id
END