-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_update_card_numbers_bikey] 
	@card_list AS dbo.key_value_array READONLY,
	@product_list AS dbo.bikey_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
		BEGIN TRY 

			--This section helps with creating the card_index, instead of calling the fuction each time
			--Which slows down the insers, we get the key and then just encrypt
			DECLARE @objid int = object_id('cards'),		
					@key varbinary(100)
			SET @key = null
			SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
			FROM mac_index_keys 
			WHERE table_id = @objid
		
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				UPDATE [cards]
				SET	[cards].card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,cardarray.[value])),
					[cards].card_index = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(cardarray.[value], 4))) + @key )),
					[cards].card_production_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),SYSDATETIMEOFFSET()))
				FROM [cards] INNER JOIN @card_list cardarray			
					ON [cards].card_id = cardarray.[key]

				UPDATE [integration_cardnumbers]
				SET [integration_cardnumbers].card_sequence_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),productarray.[value]))
				FROM [integration_cardnumbers] INNER JOIN @product_list productarray
					ON [integration_cardnumbers].product_id = productarray.[key1]
						AND [integration_cardnumbers].sub_product_id = COALESCE(productarray.[key2], -1)

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
				
			COMMIT TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
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