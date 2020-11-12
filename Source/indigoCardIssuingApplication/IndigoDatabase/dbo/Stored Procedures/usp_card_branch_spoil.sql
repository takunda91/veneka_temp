-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_card_branch_spoil] 
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	EXEC usp_issue_card_spoil @card_id, 
							 @audit_user_id,
							 @audit_workstation,
							 @ResultCode
END