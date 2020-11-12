-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_mailer_report] 
	-- Add the parameters for the stored procedure here
	@issuer_id int = NULL,
	@branch_id int = NULL,
	@date_from DATETIMEOFFSET,
	@date_to DATETIMEOFFSET,
	@product_id int=null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @date_to = DATEADD(d, 1, @date_to)
	DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

				SELECT DISTINCT
				CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_account_number)) AS customer_account_number,				
				[pin_batch].pin_batch_reference, 
				CAST(SWITCHOFFSET([pin_batch].date_created,@UserTimezone) as datetime) as date_created,
				[issuer].issuer_name, [issuer].issuer_code, [branch].branch_name, branch.branch_code,
				issuer_product.product_code+'-'+issuer_product.product_name as product,issuer_product.product_id
		FROM [cards]
				INNER JOIN [customer_account_cards]
						ON [customer_account_cards].card_id = [cards].card_id
					INNER JOIN [customer_account] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
				INNER JOIN [pin_batch_cards]
					ON [cards].card_id = [pin_batch_cards].card_id
				INNER JOIN [pin_batch]
					ON [pin_batch_cards].pin_batch_id = [pin_batch].pin_batch_id					
				INNER JOIN [branch]
					ON [cards].branch_id = [branch].branch_id
				INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id	
		INNER JOIN [issuer_product] 
		ON [cards].product_id=[issuer_product].product_id	

		WHERE [cards].card_issue_method_id = 0
				AND [issuer].issuer_id = COALESCE(@issuer_id, [issuer].issuer_id)
				AND [branch].branch_id = COALESCE(@branch_id, [branch].branch_id)
				AND CAST(SWITCHOFFSET([pin_batch].date_created,@UserTimezone) as datetime)  >= @date_from
				AND CAST(SWITCHOFFSET([pin_batch].date_created,@UserTimezone) as datetime)  <= @date_to 
				AND [cards].product_id=COALESCE(@product_id, [cards].product_id)
		ORDER BY issuer_name, issuer_code, branch_name, branch_code, date_created,
				pin_batch_reference, customer_account_number

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END
