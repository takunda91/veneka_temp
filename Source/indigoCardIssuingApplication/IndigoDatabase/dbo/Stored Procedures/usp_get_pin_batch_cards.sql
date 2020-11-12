CREATE PROCEDURE [dbo].[usp_get_pin_batch_cards] 
	@pin_batch_id bigint,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRANSACTION [GET_PIN_BATCH_CARDS]
		BEGIN TRY 
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;
			
			SELECT 
				[dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY(cards.card_number)),6,4) AS 'card_number'
				, cards.card_request_reference AS card_reference_number
				, pin_batch_statuses.pin_batch_statuses_name
			FROM 
				[cards]
				INNER JOIN pin_batch_cards ON cards.card_id = pin_batch_cards.card_id
				INNER JOIN pin_batch_statuses ON pin_batch_cards.pin_batch_cards_statuses_id = pin_batch_statuses.pin_batch_statuses_id
			WHERE pin_batch_id = @pin_batch_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;		
			
			COMMIT TRANSACTION [GET_PIN_BATCH_CARDS]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_PIN_BATCH_CARDS]
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END