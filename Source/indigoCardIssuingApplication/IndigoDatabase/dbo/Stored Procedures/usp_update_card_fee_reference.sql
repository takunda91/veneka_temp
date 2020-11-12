-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_card_fee_reference] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@fee_reference_number varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE  f SET f.fee_reference_number = @fee_reference_number
	from [cards] as c
	INNER JOIN [fee_charged]  AS f on c.fee_id= c.fee_id
	
	WHERE card_id = @card_id
END