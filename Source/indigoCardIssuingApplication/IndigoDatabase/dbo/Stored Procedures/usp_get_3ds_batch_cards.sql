-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Lists cards for a 3D secure batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_3ds_batch_cards]
	@threed_batch_id BIGINT,
	@check_masking BIT,
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Return card details for processing
	DECLARE @mask_screen bit = [dbo].MaskScreenPAN(@audit_user_id)			
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);
		
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
		DECRYPTION BY CERTIFICATE Indigo_Certificate;

			SELECT 
				@threed_batch_id as 'threeds_batch_id'
				,CASE @check_masking
				 WHEN 1 THEN 
					CASE 
						WHEN @mask_screen = 1 THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number)),6,4) 
						ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
					END
				 ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_number))
				 END AS 'card_number'
				,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].name_on_card)) as 'name_on_card'
				,[cards].card_request_reference
				,CONVERT(DATETIME2,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date))) as 'card_expiry_date'
				,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([cards].card_expiry_date)) as 'card_expiry_date'				
				,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
				,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
				,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
				,[customer_title].customer_title_name
				,CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
			FROM [dbo].[cards] 
				INNER JOIN [dbo].[customer_account_cards] ON [dbo].[customer_account_cards].card_id = [dbo].[cards].[card_id]
				INNER JOIN [dbo].[customer_account] ON [dbo].[customer_account].customer_account_id = [dbo].[customer_account_cards].[customer_account_id]
				INNER JOIN [dbo].[customer_title] ON [dbo].[customer_title].[customer_title_id] = [dbo].[customer_account].[customer_title_id]
			WHERE [dbo].[cards].card_id IN (SELECT [card_id] FROM [dbo].[threed_secure_batch_cards] WHERE [threed_batch_id] = @threed_batch_id)
	

		CLOSE SYMMETRIC KEY Indigo_Symmetric_Key

END