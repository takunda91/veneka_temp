-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get the card count for a branch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_branch_card_count] 
	@branch_id int,
	@load_card_status_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT COUNT([load_batch_cards].card_id) AS CardCount
		FROM [cards]
			LEFT JOIN [load_batch_cards]
				ON [load_batch_cards].[card_id] = [cards].card_id
					AND [load_batch_cards].load_card_status_id = COALESCE(@load_card_status_id, [load_batch_cards].load_card_status_id)
		WHERE  [cards].branch_id = @branch_id
			
END