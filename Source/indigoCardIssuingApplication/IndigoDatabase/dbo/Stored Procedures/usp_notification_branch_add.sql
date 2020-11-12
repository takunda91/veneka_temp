-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Adds notification message for batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_notification_branch_add]
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@branch_card_statuses_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [notification_branch_outbox] (branch_message_id, added_time, channel_id, card_id, branch_card_statuses_id, card_issue_method_id, issuer_id, language_id)
	SELECT NEWID(), SYSDATETIMEOFFSET(), [notification_branch_messages].channel_id, @card_id, @branch_card_statuses_id, [cards].card_issue_method_id, [branch].issuer_id, 0
	FROM [notification_branch_messages] INNER JOIN [branch]
			ON [notification_branch_messages].issuer_id = [branch].issuer_id				
		INNER JOIN [cards]
			ON [branch].branch_id = [cards].branch_id
				AND [notification_branch_messages].card_issue_method_id = [cards].card_issue_method_id
	WHERE [cards].card_id = @card_id 
		AND [notification_branch_messages].branch_card_statuses_id = @branch_card_statuses_id
		
END