-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_outstanding_orders]
	-- Add the parameters for the stored procedure here
	@product_id int,
	@number_of_cards int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT [dist_batch].* 
	FROM [dist_batch] 
		INNER JOIN [dist_batch_status_current]
			ON [dist_batch_status_current].[dist_batch_id] = [dist_batch].[dist_batch_id]
				AND [dist_batch].dist_batch_type_id = 0
				AND [dist_batch_status_current].[dist_batch_statuses_id] = 20 
		INNER JOIN [dist_batch_cards]
			ON [dist_batch].[dist_batch_id] = [dist_batch_cards].[dist_batch_id]
		INNER JOIN [cards]
			ON [cards].[card_id] = [dist_batch_cards].[card_id]
		INNER JOIN [issuer_product]
			ON [issuer_product].[product_id] = [cards].[product_id]
				AND [issuer_product].DeletedYN = 0
				AND [issuer_product].product_load_type_id = 4
		INNER JOIN [issuer]
			ON [issuer].issuer_id = [dist_batch].[issuer_id]
				AND [issuer].issuer_status_id = 0
	WHERE [issuer_product].[product_id] = @product_id
		  AND [dist_batch].no_cards = @number_of_cards
END