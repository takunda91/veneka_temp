

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_notification_branch]
@issuer_id int ,
@branch_card_statuses_id int,
@card_issue_method_id int,
@channel_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT       notification_branch_messages.issuer_id, notification_branch_messages.card_issue_method_id, notification_branch_messages.branch_card_statuses_id,notification_branch_messages.[language_id],[channel_id],[notification_text],[subject_text],languages.language_name,from_address

FROM            notification_branch_messages INNER JOIN
                         branch_card_statuses ON notification_branch_messages.branch_card_statuses_id = branch_card_statuses.branch_card_statuses_id INNER JOIN
                         card_issue_method ON notification_branch_messages.card_issue_method_id = card_issue_method.card_issue_method_id INNER JOIN
                         issuer ON notification_branch_messages.issuer_id = issuer.issuer_id
						   inner join languages ON notification_branch_messages.language_id = languages.id
						where notification_branch_messages.issuer_id=@issuer_id 
						and notification_branch_messages.branch_card_statuses_id=@branch_card_statuses_id
						and notification_branch_messages.card_issue_method_id=@card_issue_method_id
						and notification_branch_messages.channel_id=@channel_id

END