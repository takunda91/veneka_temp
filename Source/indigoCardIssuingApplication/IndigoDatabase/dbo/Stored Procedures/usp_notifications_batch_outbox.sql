-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_notifications_batch_outbox]
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT [notification_batch_outbox].issuer_id
			, [notification_batch_outbox].batch_message_id 
			, [dist_batch_statuses_flow].user_role_id
			, [dist_batch].dist_batch_type_id
			, [dist_batch].dist_batch_reference
			, [notification_batch_outbox].dist_batch_statuses_id
			,dist_batch_statuses.dist_batch_status_name
			, [notification_batch_messages].notification_text
			, [notification_batch_messages].subject_text
			, [notification_batch_messages].channel_id
			, [notification_batch_messages].from_address
			, case when [dist_batch].branch_id <> '' then  [dist_batch].branch_id  else 0 end 
	FROM [notification_batch_outbox]
		INNER JOIN [dist_batch]
			ON [dist_batch].dist_batch_id = [notification_batch_outbox].dist_batch_id
		INNER JOIN [dist_batch_statuses_flow]
			ON [dist_batch_statuses_flow].flow_dist_batch_statuses_id = [notification_batch_outbox].dist_batch_statuses_id
				--AND [dist_batch_statuses_flow].card_issue_method_id = [dist_batch].card_issue_method_id
				--AND ([dist_batch_statuses_flow].issuer_id = [dist_batch].issuer_id OR
						--[dist_batch_statuses_flow].issuer_id = -1)
		INNER JOIN [user_roles]
			ON [user_roles].user_role_id = [dist_batch_statuses_flow].user_role_id
		INNER JOIN dist_batch_statuses
				ON dist_batch_statuses.dist_batch_statuses_id=[notification_batch_outbox].dist_batch_statuses_id
		INNER JOIN [notification_batch_messages]
			ON [notification_batch_messages].issuer_id = [notification_batch_outbox].issuer_id AND
				[notification_batch_messages].dist_batch_statuses_id = [notification_batch_outbox].dist_batch_statuses_id AND				
				[notification_batch_messages].language_id = [notification_batch_outbox].language_id AND
				[notification_batch_messages].channel_id = [notification_batch_outbox].channel_id


	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
GO