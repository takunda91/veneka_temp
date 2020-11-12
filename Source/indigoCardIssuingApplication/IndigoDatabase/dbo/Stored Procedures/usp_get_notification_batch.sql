

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_notification_batch]
@issuer_id int ,
@dist_batch_type_id int,
@dist_batch_statuses_id int,
@channel_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT        notification_batch_messages.issuer_id, notification_batch_messages.dist_batch_type_id, notification_batch_messages.dist_batch_statuses_id, notification_batch_messages.language_id, 
                         notification_batch_messages.channel_id, notification_batch_messages.notification_text, notification_batch_messages.subject_text, languages.language_name,notification_batch_messages.from_address
FROM            notification_batch_messages INNER JOIN
                         dist_batch_type ON notification_batch_messages.dist_batch_type_id = dist_batch_type.dist_batch_type_id INNER JOIN
                         dist_batch_statuses ON notification_batch_messages.dist_batch_statuses_id = dist_batch_statuses.dist_batch_statuses_id INNER JOIN
                         issuer ON notification_batch_messages.issuer_id = issuer.issuer_id INNER JOIN
                         languages ON notification_batch_messages.language_id = languages.id
						 
						where notification_batch_messages.issuer_id=COALESCE(notification_batch_messages.issuer_id, @issuer_id)
						and notification_batch_messages.dist_batch_type_id=@dist_batch_type_id
						and notification_batch_messages.dist_batch_statuses_id=@dist_batch_statuses_id
						and notification_batch_messages.channel_id=@channel_id

END