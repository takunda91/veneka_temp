


-- =============================================
-- Author:		LTladi
-- Create date: 6 March 2014
-- Description:	Create's the load batch, load batch cards, load batch status and file history within a transaction.
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_load_batch_bulk_card_request] 
	@load_batch_reference varchar(50),
	@batch_status_id int,
	@user_id bigint,	
	@load_card_status_id int,
	@load_batch_type_id int,
	@card_list AS [dbo].[load_bulk_card_request] READONLY,
	@product_fields as [dbo].[bi_key_binary_value_array] READONLY,
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
	@file_type_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
	SET QUERY_GOVERNOR_COST_LIMIT 0;

	BEGIN TRANSACTION [CREATE_LOAD_REQUEST_BATCH_TRAN]
	BEGIN TRY 

		DECLARE @RC int
		DECLARE @file_id bigint
		DECLARE @load_batch_id bigint
		DECLARE @number_of_cards int
		DECLARE @load_batch_status_id bigint

		--Inserts should happen in the following order
		--1. file_history
		--2. load_batch
		--3. load_batch_status
		--4. cards
		--5. load_batch_cards

		SET @number_of_cards = (SELECT COUNT(*) FROM @card_list)

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
					@status_date DATETIME = SYSDATETIMEOFFSET(),
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

			--Determine the number of records
			SELECT @total_records = COUNT(*) FROM @card_list

			DECLARE @temp_customer_account TABLE (					
						[temp_customer_account_id] [bigint] NOT NULL,
						[card_id] [bigint] NOT NULL)

			--Used to link the actual customer account ID with the temp customer account id
			DECLARE @temp_link_customer TABLE (	
					[customer_account_id] [bigint] NOT NULL,					
					[temp_customer_account_id] [bigint] NOT NULL)

					DECLARE @temp_cards_to_process TABLE (	
			
					[temp_customer_account_id] [bigint] NULL,
	[card_number] [varchar](20) NULL,
	[reference_number] [varchar](100) NULL,
	[branch_id] [int] NULL,
	[product_id] [int] NULL,
	[card_priority_id] [int] NULL,
	[customer_account_number] [varchar](30) NULL,
	[domicile_branch_id] [int] NULL,
	[account_type_id] [int] NULL,
	[card_issue_reason_id] [int] NULL,
	[customer_first_name] [varchar](50) NULL,
	[customer_middle_name] [varchar](50) NULL,
	[customer_last_name] [varchar](50) NULL,
	[name_on_card] [varchar](100) NULL,
	[customer_title_id] [int] NULL,
	[currency_id] [int] NULL,
	[resident_id] [int] NULL,
	[customer_type_id] [int] NULL,
	[cms_id] [varchar](50) NULL,
	[contract_number] [varchar](50) NULL,
	[idnumber] [varchar](50) NULL,
	[contact_number] [varchar](50) NULL,
	[customer_id] [varchar](50) NULL,
	[fee_waiver_YN] [bit] NULL,
	[fee_editable_YN] [bit] NULL,
	[fee_charged] [decimal](10, 4) NULL,
	[fee_overridden_YN] [bit] NULL,
	[audit_user_id] [bigint] NULL,
	[audit_workstation] [varchar](100) NULL,
	[sub_product_id] [varchar](100) NULL,
	[load_product_batch_type_id] [int] NULL,
	[already_loaded] [bit] NULL,
	RowNum [int] not null
	)

	insert into @temp_cards_to_process
	SELECT *,ROW_NUMBER() OVER (ORDER BY reference_number) AS RowNum						
								 FROM @card_list

			--Insert new card records in batches
			WHILE (@processed_records < @total_records)
			BEGIN

				SET @processed_records_end = @processed_records + @records_to_process;

				DELETE FROM @temp_customer_account
				DELETE FROM @temp_link_customer

				MERGE [cards] AS cardsTable
				USING (SELECT * from
						@temp_cards_to_process AS cards_to_process
						 WHERE cards_to_process.RowNum BETWEEN @processed_records AND @processed_records_end
								AND cards_to_process.load_product_batch_type_id IN (5, 6)) AS cardList
				ON (cardsTable.card_request_reference = cardList.reference_number AND cardsTable.product_id = cardList.product_id) 
				WHEN NOT MATCHED BY TARGET THEN
					-- INSERT into Table1:
					INSERT ( [product_id], [ordering_branch_id], [branch_id], [origin_branch_id], [card_sequence], [card_index], [card_issue_method_id]
						, [card_priority_id], card_request_reference, card_number,delivery_branch_id)
					VALUES ( cardList.product_id, cardList.branch_id, cardList.branch_id, cardList.branch_id, 0, [dbo].[MAC]('0', @objid), 0
						, cardList.card_priority_id	, cardList.reference_number
						, ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR(MAX), cardList.card_number)),cardList.branch_id)
		   	
				OUTPUT					
					cardList.temp_customer_account_id					
					, inserted.card_id					
				INTO @temp_customer_account;			
				
				--MERGE [fee_charged] AS fee ---cardList.fee_waiver_YN, cardList.fee_editable_YN, cardList.fee_charged, cardList.fee_overridden_YN
				--USING (SELECT * from
				--		@temp_cards_to_process AS cards_to_process
				--		 WHERE cards_to_process.RowNum BETWEEN @processed_records AND @processed_records_end
				--				AND cards_to_process.load_product_batch_type_id IN (5, 6)) AS cardList
				--ON (fee.fee_id = cardList.reference_number AND cardsTable.product_id = cardList.product_id) 
				--WHEN NOT MATCHED BY TARGET THEN
				--INSERT (fee_waiver_YN,fee_editable_YN,fee_charged,vat,fee_overridden_YN)
				--					VALUES(cardList.fee_waiver_YN, cardList.fee_editable_YN, cardList.fee_charged, cardList.fee_overridden_YN)
				--	SET @new_fee_id = SCOPE_IDENTITY()
				--Insert temp customers to actual table
				MERGE INTO [customer_account]
				USING (SELECT t2.*, t1.card_id
						FROM @temp_customer_account t1 INNER JOIN @card_list t2
								ON t1.temp_customer_account_id = t2.temp_customer_account_id
						) AS tca
				ON 1 = 0
				WHEN NOT MATCHED THEN
				  INSERT ([user_id],[card_issue_reason_id],[account_type_id],[currency_id]
							,[resident_id],[customer_type_id],[customer_account_number]
							,[customer_first_name],[customer_middle_name],[customer_last_name]
							,[name_on_card],[date_issued],[cms_id],[contract_number],[customer_title_id]
							,[Id_number],[contact_number],[CustomerId],[domicile_branch_id])
					VALUES ([tca].audit_user_id,[tca].[card_issue_reason_id],[tca].[account_type_id],[tca].[currency_id]
							,[tca].[resident_id],[tca].[customer_type_id]
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_account_number]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_first_name]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_middle_name]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_last_name]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[name_on_card]))
							,SYSDATETIMEOFFSET(),[tca].[cms_id],[tca].[contract_number],[tca].[customer_title_id]
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[idnumber]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[contact_number]))
							,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CONVERT(VARCHAR(MAX),[tca].[customer_id]))
							,[tca].[domicile_branch_id])			
				OUTPUT inserted.customer_account_id, tca.temp_customer_account_id
				INTO @temp_link_customer;


				MERGE INTO [customer_account_cards]
				USING (SELECT t2.*, t1.card_id
						FROM @temp_customer_account t1 INNER JOIN @card_list t2
								ON t1.temp_customer_account_id = t2.temp_customer_account_id
						) AS tca
				ON 1 = 0 
				WHEN NOT MATCHED THEN
				INSERT (customer_account_id,card_id)
				VALUES (tca.[temp_customer_account_id],tca.card_id)				
				WHEN  MATCHED THEN
				update set card_id=tca.card_id;


				--Insert Product Fields
				--INSERT INTO customer_fields (customer_account_id, product_field_id, value)
				--	SELECT [customer_account_id], [key2], [value]
				--	FROM @product_fields INNER JOIN @temp_link_customer
				--			ON temp_customer_account_id = key1

				--Update Product Image Fields
				INSERT INTO customer_fields (customer_account_id, product_field_id, value)
					SELECT tlc.[customer_account_id], pf.[key2], ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'), CAST(pf.[value] as varbinary(max)))
					FROM @product_fields pf INNER JOIN @temp_link_customer tlc
								ON tlc.temp_customer_account_id = pf.key1
							INNER JOIN product_fields
								ON pf.[key2] = product_fields.product_field_id
					WHERE product_fields.print_field_type_id = 0

				--Update Product Image Fields
				INSERT INTO customer_image_fields (customer_account_id, product_field_id, value)
					SELECT [customer_account_id], pf.[key2], pf.[value]
					FROM @product_fields pf INNER JOIN @temp_link_customer
								ON temp_customer_account_id = [key1]
							INNER JOIN product_fields
								ON pf.[key2] = product_fields.product_field_id
					WHERE product_fields.print_field_type_id = 1

				SET @processed_records = @processed_records_end
			END
			

			DECLARE @load_batch_records int = 0
			DECLARE @inserted_load_cards TABLE(load_batch_id bigint, card_id bigint, load_card_status_id int);

			--Insert into load_batch_cards, links cards to load batch
			INSERT INTO load_batch_cards ([load_batch_id], [card_id], [load_card_status_id])
			OUTPUT inserted.* INTO @inserted_load_cards
			SELECT @load_batch_id, cards.card_id, @load_card_status_id
			FROM cards 
				INNER JOIN (SELECT * FROM @card_list) as cardList
				ON cards.product_id = cardList.product_id
				AND cards.card_request_reference = cardList.reference_number
			--WHERE cards.card_request_reference IN (SELECT cardList.reference_number
			--										  FROM @card_list cardList
			--										  WHERE cardList.product_id = cards.product_id)
													  
			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					
		SELECT @load_batch_records = COUNT(*) FROM @inserted_load_cards

		IF(@number_of_cards != @load_batch_records) 
			RAISERROR (N'Records inserted for loadbatch (%i) dont match those for the load (%i).', 15, 1, @load_batch_records, @number_of_cards);

		COMMIT TRANSACTION [CREATE_LOAD_REQUEST_BATCH_TRAN]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_LOAD_REQUEST_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 

	RETURN '0'
END

