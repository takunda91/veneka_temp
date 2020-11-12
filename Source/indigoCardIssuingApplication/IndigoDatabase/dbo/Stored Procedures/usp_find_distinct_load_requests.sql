-- =============================================
-- Author:		Richard Brenchley
-- Create date: 21 October 2015
-- Description:	Find all distinct card references from load cards based on supplied card reference list
--				and returns the matched cards.
--				Cards that are attached to a load batch which has been rejected are not considered
--				duplicates in the system as they are not active. When creating a new load batch
--				a merege on the card table will happen to stop duplicate card number from entereing
--				the system.
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_distinct_load_requests] 
	-- Add the parameters for the stored procedure here
	@card_ref_list dbo.key_value_array READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT card_request_reference
	FROM [cards] 
		INNER JOIN [load_batch_cards]
			ON [cards].card_id = [load_batch_cards].card_id
		INNER JOIN [load_batch]
			ON [load_batch_cards].load_batch_id = [load_batch].load_batch_id
		INNER JOIN [load_batch_status_current]
			ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
	WHERE [load_batch_status_current].load_batch_statuses_id NOT IN (2, 3)
			AND [cards].card_request_reference IN (SELECT cardReflist.value
													FROM @card_ref_list cardReflist )

END