USE [{DATABASE_NAME}]
GO

CREATE TABLE [dbo].[integration_bellid_batch_sequence] (
    [file_generation_date]  DATE     NOT NULL,
    [batch_sequence_number] SMALLINT NOT NULL,
    CONSTRAINT [PK_integration_bellid_batch_sequence] PRIMARY KEY CLUSTERED ([file_generation_date] ASC, [batch_sequence_number] ASC)
)

GO
CREATE TABLE [dbo].[rswitch_crf_bank_codes] (
    [bank_id]   INT         IDENTITY (1, 1) NOT NULL,
    [issuer_id] INT         NOT NULL,
    [bank_code] VARCHAR (2) NULL,
    CONSTRAINT [PK_bank] PRIMARY KEY CLUSTERED ([bank_id] ASC)
)

GO
ALTER TABLE [dbo].[rswitch_crf_bank_codes] WITH NOCHECK
    ADD CONSTRAINT [FK_rswitch_crf_bank_codes_issuer] FOREIGN KEY ([issuer_id]) REFERENCES [dbo].[issuer] ([issuer_id])

GO
ALTER TABLE [dbo].[rswitch_crf_bank_codes] WITH CHECK CHECK CONSTRAINT [FK_rswitch_crf_bank_codes_issuer];

GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_integration_bellid_get_sequence] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @seq_no smallint,
			@current_date date

	SET @current_date = GETDATE()

    -- Insert statements for procedure here
	SELECT @seq_no = batch_sequence_number
	FROM [integration_bellid_batch_sequence]
	WHERE file_generation_date = @current_date

	IF @seq_no IS NULL
		BEGIN
			--Start of a new seqence for today.
			SET @seq_no = 1
			INSERT INTO [integration_bellid_batch_sequence] (file_generation_date, batch_sequence_number)
				VALUES (@current_date, @seq_no)
		END
	ELSE
		BEGIN
			--BECAUSE there is already a sequence for today, increment it and update the table.
			SET @seq_no = @seq_no + 1
			UPDATE [integration_bellid_batch_sequence]
			SET batch_sequence_number = @seq_no
			WHERE file_generation_date = @current_date
		END

	SELECT @seq_no

END
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_rswitch_hsm_pin_printed] 
	@dist_batch_id bigint,
	@card_id bigint,
	@pvv varchar(10),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_PIN_PRINTED]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

				UPDATE [cards]
				SET pvv = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@pvv))
				WHERE card_id = @card_id

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			UPDATE [dist_batch_cards]  
			SET dist_card_status_id = 17
			WHERE card_id = @card_id
				AND dist_batch_id = @dist_batch_id

		COMMIT TRANSACTION [UPDATE_PIN_PRINTED]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [UPDATE_PIN_PRINTED]
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
GO

-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_rswitch_update_card_numbers] 
	@card_list AS dbo.key_value_array READONLY,
	@product_list AS dbo.key_value_array READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@activation_date DATETIME OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [UPDATE_CARD_NUMBER_TRAN]
		BEGIN TRY 

			SET @activation_date = GETDATE()
		
			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate;

				UPDATE [cards]
				SET	[cards].card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),cardarray.[value])),
					[cards].card_activation_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@activation_date)),
					[cards].card_expiry_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),DATEADD(mm, [issuer_product].expiry_months, @activation_date))),
					[cards].card_production_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar(max),@activation_date))
				FROM [cards] INNER JOIN @card_list cardarray			
						ON [cards].card_id = cardarray.[key]
					INNER JOIN [issuer_product]
						ON [cards].product_id = [issuer_product].product_id

				UPDATE [integration_cardnumbers]
				SET [integration_cardnumbers].card_sequence_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(varchar,productarray.[value]))
				FROM [integration_cardnumbers] INNER JOIN @product_list productarray
					ON [integration_cardnumbers].product_id = productarray.[key]

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
GO