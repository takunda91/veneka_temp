-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_notifications_branch_outbox]
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

	SELECT [notification_branch_outbox].issuer_id
			, [notification_branch_outbox].branch_message_id 
			, [cards].card_request_reference
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].Id_number)) as 'id_number'
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
			, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
			, [customer_title_language].language_text AS 'customer_title_name'
			, [notification_branch_messages].notification_text
			, [notification_branch_messages].subject_text
			, [notification_branch_messages].channel_id
	FROM [notification_branch_outbox]
		INNER JOIN [cards]
			ON [cards].card_id = [notification_branch_outbox].card_id		
		INNER JOIN [notification_branch_messages]
			ON [notification_branch_messages].issuer_id = [notification_branch_outbox].issuer_id AND
				[notification_branch_messages].branch_card_statuses_id = [notification_branch_outbox].branch_card_statuses_id AND
				[notification_branch_messages].card_issue_method_id = [notification_branch_outbox].card_issue_method_id AND
				[notification_branch_messages].language_id = [notification_branch_outbox].language_id AND
				[notification_branch_messages].channel_id = [notification_branch_outbox].channel_id
		LEFT OUTER JOIN [customer_account_cards]
			ON [customer_account_cards].card_id = [notification_branch_outbox].card_id
	 inner join customer_account 
			ON [customer_account_cards].Customer_account_id = customer_account.Customer_account_id
		LEFT OUTER JOIN [customer_title_language]
			ON [customer_title_language].customer_title_id = [customer_account].customer_title_id
				AND [customer_title_language].language_id = [notification_branch_outbox].language_id

	CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
END