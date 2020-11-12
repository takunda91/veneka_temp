-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Get the card count for a card centre
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_card_centre_card_count] 
	@branch_id int,
	@product_id int,
	--@sub_product_id int = NULL,
	@card_issue_method_id int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @card_centre_count int = 0,
			@branch_count int = 0,
			@load_count int = 0

	

	SELECT @card_centre_count = COUNT(DISTINCT [cards].card_id)
	FROM [cards]
			INNER JOIN [avail_cc_and_load_cards]
				ON [cards].card_id = [avail_cc_and_load_cards].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
	WHERE [cards].branch_id = @branch_id
			AND [cards].product_id = @product_id
			--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
			AND [cards].card_issue_method_id = @card_issue_method_id
			AND [branch].branch_type_id = 0
	

	SELECT @branch_count = COUNT(DISTINCT [cards].card_id)
	FROM [cards]
			INNER JOIN [branch_card_status_current]
				ON [cards].card_id = [branch_card_status_current].card_id
			INNER JOIN [branch]
				ON [cards].branch_id = [branch].branch_id
	WHERE [cards].branch_id = @branch_id
			AND [cards].product_id = @product_id
			--AND [cards].sub_product_id = @sub_product_id			
			AND [branch_card_status_current].branch_card_statuses_id = 0
			AND [cards].card_issue_method_id = @card_issue_method_id
			AND [branch].branch_type_id != 0


	SELECT (@card_centre_count + @branch_count + @load_count) as CardCount
END
