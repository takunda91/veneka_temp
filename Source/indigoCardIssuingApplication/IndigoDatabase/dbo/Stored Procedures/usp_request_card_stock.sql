-- =============================================
-- Author:	Nduvho Mukhavhuli
-- Create date: 2014/09/25
-- Description:	Creates a new card order
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_card_stock] 
	@issuer_id int,
	@branch_id int,
	@product_id int,	
	@card_priority_id int,
	@card_issue_method_id int,	
	@cards_in_batch int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@dist_batch_id bigint OUTPUT,
	@dist_batch_ref varchar(50) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [CREATE_CARDS_STOCK]
		BEGIN TRY 

			OPEN SYMMETRIC KEY Indigo_Symmetric_Key
			DECRYPTION BY CERTIFICATE Indigo_Certificate

			DECLARE @status_date datetimeoffset
			DECLARE @card_id bigint,
					@newGuid uniqueidentifier

			DECLARE @objid int
			SET @objid = object_id('cards')
			SET @status_date = SYSDATETIMEOFFSET()


			--create the production batch
			INSERT INTO [dist_batch]
				([card_issue_method_id],[issuer_id], [branch_id], [no_cards],[date_created],[dist_batch_reference],[dist_batch_type_id])
			VALUES (@card_issue_method_id, @issuer_id, @branch_id, @cards_in_batch, @status_date, @status_date, 0)

			SET @dist_batch_id = SCOPE_IDENTITY();

			--add prod batch status of created
			INSERT INTO [dbo].[dist_batch_status]
				([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
			VALUES(@dist_batch_id, 0, @audit_user_id, @status_date, 'Batch Created')

			--Generate prod batch reference
			SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										CONVERT(VARCHAR(8), @status_date, 112) + '' +
										CAST(@dist_batch_id AS varchar(max))
			FROM [issuer]					
				INNER JOIN [issuer_product]
					ON [issuer_product].issuer_id = [issuer].issuer_id
			WHERE [issuer_product].product_id = @product_id

			--UPDATE prod batch with reference and number of cards
			UPDATE [dist_batch]
			SET [dist_batch_reference] = @dist_batch_ref,
				[no_cards] = @cards_in_batch
			WHERE [dist_batch].dist_batch_id = @dist_batch_id

			--This section helps with creating the card_index, instead of calling the fuction each time
			--Which slows down the insers, we get the key and then just encrypt
			SET @objid = object_id('cards')			
			DECLARE @key varbinary(100)
			SET @key = null
			SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
			FROM mac_index_keys 
			WHERE table_id = @objid

			IF(@key IS NULL)
				RAISERROR (N'MAC Index Key is null.', 10, 1);

			--Table for storing new card id's
			Declare @inserted_cards TABLE (	card_id bigint )

			--Create Cards for the Batch
			DECLARE @index int = 0			
			WHILE @index < @cards_in_batch
			BEGIN
				SET @newGuid = NEWID();

				--Inserting a card record with an empty card number, the card number will be generated later in the process. 
				-- when that happens this record should be populated with a card number.
				INSERT INTO [cards]	([product_id], [ordering_branch_id], [branch_id], [delivery_branch_id],[origin_branch_id],[card_number],[card_sequence],
										[card_issue_method_id], [card_priority_id], [card_request_reference], [card_index]) 
					OUTPUT Inserted.card_id INTO @inserted_cards
					VALUES(@product_id, @branch_id, @branch_id, @branch_id, @branch_id, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(max), @newGuid)), 0,
						   --[dbo].[MAC]('0', @objid), 
						   @card_issue_method_id, @card_priority_id
						   ,CONVERT(varchar(100), @newGuid)
						   ,CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000), @newGuid)) + @key )))

				--SET @card_id = SCOPE_IDENTITY();

				SET @index = @index +1
			END			

			--UPDATE CARDS WITH UNIQUE REF
			UPDATE [cards]
				SET card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR, dbo.GenCardReferenceNo(@status_date, [cards].card_id))),
					card_request_reference = dbo.GenCardReferenceNo(@status_date, [cards].card_id),
					card_index = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000), RIGHT(dbo.GenCardReferenceNo(@status_date, [cards].card_id),4))) + @key ))
			WHERE [card_id] IN (SELECT card_id FROM @inserted_cards)

			--LINK CARDS TO THE BATCH
			INSERT INTO [dist_batch_cards] (card_id, dist_batch_id, dist_card_status_id)
				SELECT card_id, @dist_batch_id, 12 FROM @inserted_cards

			--UPDATE CARDS WITH UNIQUE REF
			--UPDATE [cards]
			--		SET [cards].card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),
			--									CONVERT(VARCHAR, dbo.GenCardReferenceNo(@status_date, [cards].card_id))),
			--			[cards].card_request_reference = dbo.GenCardReferenceNo(@status_date, [cards].card_id)
			--FROM [cards]
			--		INNER JOIN [dist_batch_cards]
			--			ON [cards].card_id = [dist_batch_cards].card_id
			--WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id

			EXEC usp_notification_batch_add @dist_batch_id, 0		

			--Add audit for dist batch creation	
			DECLARE @dist_batch_status_name varchar(50),
					@audit_msg nvarchar(500)
			SELECT @dist_batch_status_name =  dist_batch_status_name
			FROM dist_batch_statuses
			WHERE dist_batch_statuses_id = 0
											
			SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
								', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
								', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
			--log the audit record		
			EXEC usp_insert_audit @audit_user_id, 
									2,
									NULL, 
									@audit_workstation, 
									@audit_msg, 
									NULL, NULL, NULL, NULL

			SET @ResultCode = 0


			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key

			COMMIT TRANSACTION [CREATE_CARDS_STOCK]			
		END TRY

		BEGIN CATCH
			ROLLBACK TRANSACTION [CREATE_CARDS_STOCK]
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