-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_dist_batch_to_vault] 
	-- Add the parameters for the stored procedure here
	@dist_batch_id bigint, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @branch_card_status_id int = 0

	--Update the cards linked to the dist batch with the new status.
	UPDATE dist_batch_cards
	SET dist_card_status_id = 2
	WHERE dist_batch_id = @dist_batch_id

	--Insert cards into branch status as checked in.
	-- First check if the card has a branch card status
	INSERT INTO branch_card_status_audit ([card_id], [branch_card_statuses_id], [status_date], [user_id], [operator_user_id], [branch_card_code_id], [comments], [pin_auth_user_id], [branch_id])
	SELECT [card_id], [branch_card_statuses_id], [status_date], [user_id], [operator_user_id], [branch_card_code_id], [comments], [pin_auth_user_id], [branch_id] 
	FROM 
	(
		MERGE branch_card_status AS t
		USING (	SELECT [cards].branch_id, [dist_batch_cards].card_id, @branch_card_status_id AS branch_card_status_id, SYSDATETIMEOFFSET() AS status_date, @audit_user_id AS audit_user_id
				FROM [dist_batch_cards] INNER JOIN [cards]
					ON [cards].card_id = [dist_batch_cards].card_id
				WHERE dist_batch_id = @dist_batch_id)s
		ON t.card_id = s.card_id
		WHEN MATCHED THEN
			UPDATE 
			SET branch_id = s.branch_id, 
				branch_card_statuses_id = s.branch_card_status_id, 
				status_date = s.status_date, 
				[user_id] = s.audit_user_id,
				operator_user_id = NULL,
				branch_card_code_id = NULL,
				comments = NULL,
				pin_auth_user_id = NULL
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (branch_id, card_id, branch_card_statuses_id, status_date, [user_id])
				VALUES (s.branch_id, s.card_id, s.branch_card_status_id, s.status_date, s.audit_user_id)
		OUTPUT $action as act, Deleted.*) as audited
	WHERE  act = 'UPDATE';

	--INSERT branch_card_status(branch_id, card_id, branch_card_statuses_id, status_date, [user_id])
	--SELECT [cards].branch_id, [dist_batch_cards].card_id, @branch_card_status_id, SYSDATETIMEOFFSET(), @audit_user_id
	--FROM [dist_batch_cards] INNER JOIN [cards]
	--	ON [cards].card_id = [dist_batch_cards].card_id
	--WHERE dist_batch_id = @dist_batch_id

	--Insert notifcations for cards at branch
	INSERT INTO [notification_branch_outbox] (branch_message_id, added_time, channel_id, card_id, branch_card_statuses_id, card_issue_method_id, issuer_id, language_id)
	SELECT NEWID(), SYSDATETIMEOFFSET(), [notification_branch_messages].channel_id, [cards].card_id, @branch_card_status_id, [cards].card_issue_method_id, [branch].issuer_id, 0
	FROM [notification_branch_messages] INNER JOIN [branch]
			ON [notification_branch_messages].issuer_id = [branch].issuer_id
				AND [notification_branch_messages].branch_card_statuses_id = @branch_card_status_id		
		INNER JOIN [cards]
			ON [branch].branch_id = [cards].branch_id
				AND [notification_branch_messages].card_issue_method_id = [cards].card_issue_method_id
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
	WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
		

	--Update the originating branch to the current branch.
	UPDATE [cards]
	SET origin_branch_id = [cards].branch_id
	FROM [cards]
		INNER JOIN [dist_batch_cards]
			ON [cards].card_id = [dist_batch_cards].card_id
	WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id 

END