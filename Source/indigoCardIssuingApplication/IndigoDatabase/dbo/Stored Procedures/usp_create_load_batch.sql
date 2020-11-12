-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Create's the load batch, load batch cards, load batch status and file history within a transaction.
-- =============================================
Create PROCEDURE [dbo].[usp_create_load_batch] 
	@load_batch_reference varchar(50),
	@batch_status_id int,
	@user_id bigint,	
	@load_card_status_id int,
	--@card_list AS dbo.load_cards_type READONLY,
	@issuer_id int,
	@file_load_id INT,
	@name_of_file varchar(60),
	@file_created_date datetimeoffset,
	@file_size int,
	@load_date datetimeoffset,
	@file_status_id int,
	@file_directory varchar(110),
	@number_successful_records int,
	@number_failed_records int,
	@file_load_comments varchar(max),
	@file_type_id int,
	@load_batch_type_id int,
	@order_batch_id bigint = NULL,
	@file_id bigint = NULL OUTPUT,
	@load_batch_id bigint = NULL OUTPUT,
	@load_batch_status_id bigint = NULL OUTPUT


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
	SET QUERY_GOVERNOR_COST_LIMIT 0;

	BEGIN TRANSACTION [CREATE_LOAD_BATCH_TRAN]
	BEGIN TRY 

		DECLARE @RC int
		DECLARE @number_of_cards int

		--Inserts should happen in the following order
		--1. file_history
		--2. load_batch
		--3. load_batch_status
		--4. cards
		--5. load_batch_cards

		SET @number_of_cards = (SELECT COUNT(*) FROM [temp_load_cards_type])

		--Insert into file history
		EXECUTE @RC = [dbo].[usp_insert_file_history] @file_load_id, @issuer_id,@name_of_file,@file_created_date
													,@file_size,@load_date,@file_status_id,@file_directory
													,@number_successful_records,@number_failed_records
													,@file_load_comments,@file_type_id,@file_id OUTPUT

		--Insert into load_batch
		EXECUTE @RC = [dbo].[usp_insert_load_batch] @load_batch_reference,@file_id,@issuer_id,@batch_status_id
												  ,@load_date,@number_of_cards,@load_batch_type_id, @load_batch_id OUTPUT

		--Insert into load_batch_status
		EXECUTE @RC = [dbo].[usp_insert_load_batch_status] @load_batch_id,@batch_status_id,@load_date
														 ,@user_id,@load_batch_status_id OUTPUT

		
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
		DECRYPTION BY CERTIFICATE Indigo_Certificate
			DECLARE @objid int,
					@total_records int,
					@processed_records int = 0,
					@status_date DATETIME = GETDATE(),
					@processed_records_end int,
					@records_to_process int = 500 --Use this to tune the amount of records to load	

			--This section helps with creating the card_index, instead of calling the fuction each time
			--Which slows down the insers, we get the key and then just encrypt
			SET @objid = object_id('cards')			
			DECLARE @key varbinary(100)
			SET @key = null
			SELECT @key = DecryptByKeyAutoCert(cert_id('cert_ProtectIndexingKeys'), null, mac_key) 
			FROM mac_index_keys 
			WHERE table_id = @objid

			IF(@key IS NULL)
				RAISERROR (N'MAC Index Key is null.', 15, 1);

			--Determin the number of records
			SELECT @total_records = COUNT(*) FROM [temp_load_cards_type]

			--table car for storing the new card id's. used later when creating the ref numbers.
			DECLARE @new_card_id_list TABLE ( card_id bigint)

			--Insert new card records in batches
			WHILE (@processed_records < @total_records)
			BEGIN

				SET @processed_records_end = @processed_records + @records_to_process;

				MERGE [cards] AS cardsTable
				USING (SELECT *
						FROM (
								SELECT *, 
								ROW_NUMBER() OVER (ORDER BY card_number) AS RowNum
								 FROM [temp_load_cards_type]								 
								 ) AS cards_to_process
						 WHERE cards_to_process.RowNum BETWEEN @processed_records AND @processed_records_end
								AND cards_to_process.load_batch_type_id IN (1, 2, 3))
								
						AS cardsList
				ON ((DECRYPTBYKEY(cardsTable.card_number) = cardsList.card_number or cardsTable.card_request_reference = cardsList.card_reference ) AND cardsTable.product_id = cardsList.product_id) 
				 WHEN MATCHED    THEN  
				 					
				UPDATE SET [product_id]=cardsList.product_id,
							[card_issue_method_id]=cardsList.card_issue_method_id,
							[ordering_branch_id]=cardsList.branch_id,
							[branch_id]=cardsList.branch_id,
							[origin_branch_id]=cardsList.branch_id,
							[delivery_branch_id]=cardsList.branch_id,
							[card_number]=COALESCE(NULL,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(100),cardsList.card_number)),cardsTable.[card_number]),
							[card_sequence]=COALESCE(NULL,cardsList.card_sequence,cardsTable.[card_sequence]),
							[card_priority_id]=1,
							[card_request_reference]=COALESCE(NULL,cardsList.card_reference,[card_request_reference]),
							[card_expiry_date]=COALESCE(NULL,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(100),cardsList.[expiry_date])),[card_expiry_date]),
							[card_index]=COALESCE(NULL,CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(cardsList.card_number, 4))) + @key )),[card_index])
				WHEN NOT MATCHED BY TARGET
					THEN INSERT ([product_id],[card_issue_method_id]
									,[ordering_branch_id],[branch_id],[origin_branch_id],[delivery_branch_id]
									,[card_number]
									,[card_sequence],[card_priority_id],[card_request_reference]
									,[card_expiry_date]
									,[card_index]) 
						VALUES(cardsList.product_id
							   --cardsList.sub_product_id,
							   ,cardsList.card_issue_method_id
							   ,cardsList.branch_id
							   ,cardsList.branch_id
							   ,cardsList.branch_id
							   ,cardsList.branch_id
							   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,cardsList.card_number))
							   ,cardsList.card_sequence
							   ,1
							   ,cardsList.card_reference
							   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,cardsList.[expiry_date]))
							   ,CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(cardsList.card_number, 4))) + @key )))
				OUTPUT inserted.card_id INTO @new_card_id_list;
				
				--Update with card reference number
				--UPDATE [cards]
				--	SET card_request_reference = dbo.GenCardReferenceNo(@status_date, [cards].card_id)
				--WHERE [card_id] IN (SELECT card_id FROM @new_card_id_list)

				DELETE FROM @new_card_id_list




				SET @processed_records = @processed_records_end
			END

			DECLARE @load_batch_records int = 0

			--Update ordered cards
			UPDATE cardsupdate
			SET cardsupdate.card_number = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,card_list.card_number)),
				cardsupdate.card_expiry_date = ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,card_list.[expiry_date])),
				cardsupdate.card_request_reference = ISNULL(card_list.card_reference, cardsupdate.card_request_reference),
				cardsupdate.card_sequence = card_list.card_sequence,
				cardsupdate.branch_id = COALESCE(card_list.branch_id, cardsupdate.branch_id),
				cardsupdate.origin_branch_id = COALESCE(card_list.branch_id, cardsupdate.origin_branch_id),
				cardsupdate.card_index = CONVERT(varbinary(24),HashBytes( N'SHA1', CONVERT(varbinary(8000), CONVERT(nvarchar(4000),RIGHT(card_list.card_number, 4))) + @key ))
			FROM [temp_load_cards_type] card_list
					INNER JOIN [cards] cardsupdate
						ON cardsupdate.card_id = card_list.card_id
			WHERE card_list.load_batch_type_id = 4


			DECLARE @inserted_load_cards TABLE(load_batch_id bigint, card_id bigint, load_card_status_id int);

			--Insert into load_batch_cards, links cards to load batch
			INSERT INTO load_batch_cards ([load_batch_id], [card_id], [load_card_status_id])
			OUTPUT inserted.* INTO @inserted_load_cards
			SELECT @load_batch_id, cards.card_id, @load_card_status_id
			FROM cards 
			WHERE cards.product_id IN (SELECT DISTINCT cards_list.product_id FROM [temp_load_cards_type] cards_list)
			AND DECRYPTBYKEY(cards.card_number) IN (SELECT cards_list.card_number FROM [temp_load_cards_type] cards_list)
			--	INNER JOIN (SELECT * FROM @card_list) as cardList
			--	ON cards.product_id = cardList.product_id
			--	AND DECRYPTBYKEY(cards.card_number) = cardList.card_number 
			--WHERE DECRYPTBYKEY(cards.card_number) IN (SELECT cards_list.card_number
			--										  FROM @card_list cards_list
			--										  WHERE cards_list.product_id = cards.product_id)

			--SELECT @load_batch_records = @@ROWCOUNT

				-- Update the production batch from load pending to load pending approval
			IF(@order_batch_id IS NOT NULL)
			BEGIN
				UPDATE [dist_batch_status]
				SET [dist_batch_statuses_id] = 23, 
					[user_id] = -2, 
					[status_date] = SYSDATETIMEOFFSET(), 
					[status_notes] = 'Cards loaded pending approval'	
				OUTPUT Deleted.* INTO dist_batch_status_audit
				WHERE [dist_batch_id] = @order_batch_id
			END
			--SELECT @load_batch_records = @@ROWCOUNT

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					
			--declare @audit_msg nvarchar(50)
			--SET @audit_msg = 'INSERT :' + CAST(@load_batch_reference AS varchar(max))  +', LOADED'
			----log the audit record		
			--EXEC usp_insert_audit @user_id, 
			--						5, --LoadBatch
			--						NULL, 
			--						'LOADED', 
			--						@audit_msg, 
			--						NULL, NULL, NULL, NULL		

			SELECT @load_batch_records = COUNT(*) FROM @inserted_load_cards

			

		IF(@number_of_cards != @load_batch_records) --@load_batch_records)
		begin
			RAISERROR (N'Records inserted for loadbatch (%i) dont match those for the load (%i).', 15, 1, @load_batch_records, @number_of_cards);
			end 
			else 
			begin
				update [dbo].[fileloader_status] set [fileloader_status]=1,[executed_datetime] = SYSDATETIMEOFFSET()
			end
		COMMIT TRANSACTION [CREATE_LOAD_BATCH_TRAN]
		update [dbo].[fileloader_status] set [fileloader_status]=2,[executed_datetime] = SYSDATETIMEOFFSET()
		delete  from temp_load_cards_type
	END TRY
	BEGIN CATCH
		update [dbo].[fileloader_status] set [fileloader_status]=0,[executed_datetime] = SYSDATETIMEOFFSET()
		delete  from temp_load_cards_type
		ROLLBACK TRANSACTION [CREATE_LOAD_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 

	RETURN '0'
END



