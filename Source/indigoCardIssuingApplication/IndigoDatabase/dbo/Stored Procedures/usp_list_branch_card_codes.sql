-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_list_branch_card_codes] 
	-- Add the parameters for the stored procedure here
	@branch_card_code_type_id int = NULL,
	@is_exception bit = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM [branch_card_codes]
	WHERE branch_card_code_type_id = COALESCE(@branch_card_code_type_id, branch_card_code_type_id)
		  AND is_exception = COALESCE(@is_exception, is_exception)
		  AND branch_card_code_enabled = 1
END