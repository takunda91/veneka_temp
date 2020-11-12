-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_customer_details] 
	-- Add the parameters for the stored procedure here
	@card_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
			Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

    OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		SELECT CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_account_number)) as customer_account_number,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_first_name)) as customer_first_name,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_middle_name)) as customer_middle_name,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(customer_last_name)) as customer_last_name,
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(name_on_card)) as name_on_card,
			   domicile_branch_id,
			   [customer_account].customer_account_id as'customer_account_id', 
			   [user_id], 
			   [customer_account_cards].card_id AS 'card_id', 
			   card_issue_reason_id, 
			   account_type_id,
			   currency_id, 
			   resident_id, 
			   customer_type_id, 
				CAST(SWITCHOFFSET( date_issued,@UserTimezone) as DateTime) as 'date_issued' 
			   , cms_id, 
			   contract_number, 
			   customer_title_id , 
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(Id_number)) as 'Id_number',
			   CONVERT(VARCHAR(max),DECRYPTBYKEY(contact_number)) as contact_number
		FROM [customer_account]  
					INNER JOIN [customer_account_cards] ON [customer_account].customer_account_id =[customer_account_cards].customer_account_id
		WHERE [customer_account_cards].card_id = @card_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END