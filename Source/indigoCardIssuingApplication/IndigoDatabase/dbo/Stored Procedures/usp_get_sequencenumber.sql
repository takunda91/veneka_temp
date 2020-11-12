-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_sequencenumber]
	@product_id bigint,
	@sub_product_id int = NULL,
	@auditUserId int ,
	@auditWorkStation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (@sub_product_id IS NULL)
		SET @sub_product_id = -1

	BEGIN TRANSACTION [GET_CARD_SEQ_TRAN]
		BEGIN TRY 
		
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

			--check if the product has a sequence number, if it doesnt add the new sequence
			IF ((SELECT COUNT(*) FROM integration_cardnumbers WHERE product_id = @product_id AND
					sub_product_id = @sub_product_id) = 0)
				BEGIN
					INSERT INTO integration_cardnumbers (product_id, sub_product_id, card_sequence_number)
					VALUES (@product_id, @sub_product_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,0)))
				END			

			--Grab the latest sequence number for the product
			SELECT TOP 1 CONVERT(VARCHAR(max),DECRYPTBYKEY(card_sequence_number))
			FROM	integration_cardnumbers
			WHERE	product_id = @product_id
				AND	sub_product_id = @sub_product_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key
				
			COMMIT TRANSACTION [GET_CARD_SEQ_TRAN]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_CARD_SEQ_TRAN]
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