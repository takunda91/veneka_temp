-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Registeres a new batch for unregistered cards and returns the cards in that batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_3ds_unregistered_cards]
	@issuer_id INT,
	@interface_guid NCHAR(36), 	
	@check_masking BIT,
	@language_id INT,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @card_count int = 0

	DECLARE @cards TABLE ( card_id bigint not null )

	-- Check if there are any issued cards that have not been added to a 3d secure batch for registration
	INSERT INTO @cards (card_id)
	SELECT [dbo].[cards].[card_id]	
	FROM [dbo].[cards]
		INNER JOIN [dbo].[branch_card_status_current] ON [dbo].[branch_card_status_current].[card_id] = [dbo].[cards].[card_id]
		INNER JOIN [dbo].[issuer_product] ON [dbo].[issuer_product].[product_id] = [dbo].[cards].[product_id]
		INNER JOIN [dbo].[product_interface] ON [dbo].[product_interface].[product_id] = [dbo].[issuer_product].[product_id]    
	WHERE 
		[dbo].[branch_card_status_current].[branch_card_statuses_id] = 6
		AND [dbo].[issuer_product].[issuer_id] = @issuer_id
		AND [dbo].[product_interface].[interface_guid] = @interface_guid
		--AND [dbo].[product_interface].[connection_parameter_id] = @config_id
		AND [dbo].[cards].[card_id] NOT IN (SELECT [card_id] FROM [dbo].[threed_secure_batch_cards])


	SELECT @card_count = COUNT(card_ID) FROM @cards

	-- no cards for registration, exit the stored proc
	IF(@card_count <= 0)
	BEGIN
		RETURN 0
	END

	BEGIN TRANSACTION [GET_3DS_UNREGISTERED_CARDS]
	BEGIN TRY 
		
		DECLARE @threed_batch_id bigint,
				@status_date datetimeoffset = SYSDATETIMEOFFSET()

		-- Create a new 3DS batch
		INSERT INTO [dbo].[threed_secure_batch] ([date_created], [batch_reference], [issuer_id], [no_cards])
		VALUES (@status_date, @status_date, @issuer_id, @card_count)

		SET @threed_batch_id = SCOPE_IDENTITY();

		-- Add status of created
		INSERT INTO [dbo].[threed_secure_batch_status] ([threed_batch_id], [threed_batch_statuses_id], [user_id], [status_date], [status_note])
		VALUES (@threed_batch_id, 0, @audit_user_id, @status_date, 'SYSTEM CREATED BATCH')

		-- Add cards to the batch
		INSERT INTO [dbo].[threed_secure_batch_cards] ([threed_batch_id], [card_id])
		SELECT @threed_batch_id, card_id
		FROM @cards

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

		--AUDIT 
		DECLARE @batch_status_name varchar(100),
				@batch_ref varchar(100),
				@audit_msg varchar(max)

		SELECT @batch_status_name =  threed_batch_statuses_name
		FROM threed_secure_batch_statuses
		WHERE threed_batch_statuses_id = 0

		SELECT @batch_ref = batch_reference
		FROM threed_secure_batch
		WHERE threed_batch_id = @threed_batch_id

		--Add audit for pin batch update								
		SET @audit_msg = 'CREATE: ' + CAST(@threed_batch_id AS varchar(max)) +
							', ' + COALESCE(@batch_ref, 'UNKNOWN') +
							', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
		--log the audit record		
		EXEC usp_insert_audit @audit_user_id, 
								12,
								NULL, 
								@audit_workstation, 
								@audit_msg, 
								NULL, NULL, NULL, NULL


		COMMIT TRANSACTION [GET_3DS_UNREGISTERED_CARDS]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [GET_3DS_UNREGISTERED_CARDS]
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