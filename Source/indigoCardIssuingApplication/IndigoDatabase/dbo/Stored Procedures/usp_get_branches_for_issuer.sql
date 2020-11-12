-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get a branchs by issuer Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branches_for_issuer] 
	-- Add the parameters for the stored procedure here
	@issuer_id int,
	@branch_type_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT        branch.branch_id, branch.branch_status_id, branch.issuer_id, branch.branch_code, branch.branch_name, branch.location, branch.contact_person, branch.contact_email, branch.card_centre, 
                         branch.emp_branch_code, branch.branch_type_id, branch.main_branch_id
FROM            branch 
	WHERE issuer_id = @issuer_id
	AND branch_status_id = 0
	AND (branch.branch_type_id =COALESCE( @branch_type_id,branch.branch_type_id))

	--AND card_centre_branch_YN = COALESCE(@card_centre_branch, card_centre_branch_YN)
END