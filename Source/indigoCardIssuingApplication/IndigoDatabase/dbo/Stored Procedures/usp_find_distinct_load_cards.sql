-- =============================================
-- Author:		Richard Brenchley
-- Create date: 5 April 2014
-- Description:	Find all distinct card numbers from load cards based on supplied card numbers list
--				and returns the matched cards.
--				Cards that are attached to a load batch which has been rejected are not considered
--				duplicates in the system as they are not active. When creating a new load batch
--				a merege on the card table will happen to stop duplicate card number from entereing
--				the system.
-- =============================================
CREATE PROCEDURE [dbo].[usp_find_distinct_load_cards] 
	-- Add the parameters for the stored procedure here
	@card_list dbo.DistBatchCards READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT DISTINCT [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4)
		FROM [cards] 
			INNER JOIN [load_batch_cards]
				ON [cards].card_id = [load_batch_cards].card_id
			INNER JOIN [load_batch]
				ON [load_batch_cards].load_batch_id = [load_batch].load_batch_id
			INNER JOIN [load_batch_status_current]
				ON [load_batch_status_current].load_batch_id = [load_batch].load_batch_id
		WHERE [load_batch_status_current].load_batch_statuses_id NOT IN (2, 3)
			  AND CONVERT(VARCHAR(max),DECRYPTBYKEY([cards].card_number)) IN (SELECT cardlist.card_number
																		 FROM @card_list cardlist )
																		
      
	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END