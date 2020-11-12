-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_validate_cards_ordered] 
	@card_list dbo.key_value_array READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		SELECT DISTINCT card_id, product_id
		FROM [cards]
		WHERE card_id IN (
			SELECT DISTINCT [dist_batch_cards].card_id
			FROM [dist_batch_cards]
					INNER JOIN [dist_batch]
						ON [dist_batch].dist_batch_id = [dist_batch_cards].dist_batch_id
							AND [dist_batch].dist_batch_type_id = 0
					INNER JOIN [dist_batch_status_current]
						ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
			WHERE [dist_batch_cards].dist_card_status_id = 20
					AND [dist_batch_status_current].dist_batch_statuses_id = 20
					AND [dist_batch_cards].card_id IN ( SELECT cardList.[key]
														FROM @card_list cardList)
		)
END