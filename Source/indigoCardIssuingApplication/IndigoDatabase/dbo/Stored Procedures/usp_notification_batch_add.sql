-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Adds notification message for batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_notification_batch_add]
	-- Add the parameters for the stored procedure here
	@dist_batch_id bigint,
	@dist_batch_statuses_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [notification_batch_outbox] (batch_message_id, added_time, channel_id, dist_batch_id, dist_batch_type_id, dist_batch_statuses_id, issuer_id, language_id)
	SELECT NEWID(), SYSDATETIMEOFFSET(), [notification_batch_messages].channel_id, @dist_batch_id, [dist_batch].dist_batch_type_id, @dist_batch_statuses_id, [dist_batch].issuer_id, 0
	FROM [notification_batch_messages] INNER JOIN [dist_batch]
		ON [notification_batch_messages].issuer_id = [dist_batch].issuer_id
			AND [notification_batch_messages].dist_batch_type_id = [dist_batch].dist_batch_type_id
	WHERE [dist_batch].dist_batch_id = @dist_batch_id AND dist_batch_statuses_id = @dist_batch_statuses_id
END