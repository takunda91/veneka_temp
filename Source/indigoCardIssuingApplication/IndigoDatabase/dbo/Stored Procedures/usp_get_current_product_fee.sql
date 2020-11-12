
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_current_product_fee] 
	-- Add the parameters for the stored procedure here
	@fee_detail_id int,
	@currency_id int,
	@card_issue_reason_id int,
	@cbs_account_type varchar(10),
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF(@cbs_account_type = '')
	SET @cbs_account_type=NULL
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

SELECT        product_fee_charge.fee_detail_id, product_fee_charge.currency_id, product_fee_charge.card_issue_reason_id, product_fee_charge.fee_charge,CAST(SWITCHOFFSET( product_fee_charge.date_created,@UserTimezone) AS datetime) as date_created, product_fee_charge.vat
FROM            product_fee_detail INNER JOIN
                         product_fee_charge ON product_fee_detail.fee_detail_id = product_fee_charge.fee_detail_id AND product_fee_charge.card_issue_reason_id = @card_issue_reason_id AND 
                         product_fee_charge.currency_id = @currency_id
	WHERE [product_fee_detail].fee_detail_id = @fee_detail_id and product_fee_charge.cbs_account_type=COALESCE( @cbs_account_type,product_fee_charge.cbs_account_type)
	END
	GO




