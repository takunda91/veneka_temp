USE [indigo_database_main_dev]
GO
/****** Object:  StoredProcedure [dbo].[sp_create_load_batch]    Script Date: 2014/08/19 06:27:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Richard Brenchley
-- Create date: 6 March 2014
-- Description:	Create's the load batch, load batch cards, load batch status and file history within a transaction.
-- =============================================
ALTER PROCEDURE [dbo].[sp_create_load_batch] 
	@load_batch_reference varchar(50),
	@batch_status_id int,
	@user_id bigint,	
	@load_card_status_id int,
	@card_list AS dbo.load_cards_type READONLY,
	@issuer_id int,
	@file_load_id INT,
	@name_of_file varchar(60),
	@file_created_date datetime,
	@file_size int,
	@load_date datetime,
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
	SET NOCOUNT ON;
	SET QUERY_GOVERNOR_COST_LIMIT 0;

	BEGIN TRANSACTION [CREATE_LOAD_BATCH_TRAN]
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
		EXECUTE @RC = [dbo].[sp_insert_file_history] @file_load_id, @issuer_id,@name_of_file,@file_created_date
													,@file_size,@load_date,@file_status_id,@file_directory
													,@number_successful_records,@number_failed_records
													,@file_load_comments,@file_type_id,@file_id OUTPUT

		--Insert into load_batch
		EXECUTE @RC = [dbo].[sp_insert_load_batch] @load_batch_reference,@file_id,@issuer_id,@batch_status_id
												  ,@load_date,@number_of_cards,@load_batch_id OUTPUT

		--Insert into load_batch_status
		EXECUTE @RC = [dbo].[sp_insert_load_batch_status] @load_batch_id,@batch_status_id,@load_date
														 ,@user_id,@load_batch_status_id OUTPUT

		
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key		
		DECRYPTION BY CERTIFICATE Indigo_Certificate
			DECLARE @objid int
			SET @objid = object_id('cards')

			MERGE [cards] AS cardsTable
			USING @card_list AS cardsList
			ON (DECRYPTBYKEY(cardsTable.card_number) = cardsList.card_number) 
			WHEN NOT MATCHED BY TARGET
				THEN INSERT ([product_id],[branch_id],[card_number],[card_sequence],[card_index],[card_issue_method_id],[card_priority_id]) 
					VALUES(cardsList.product_id,
						   cardsList.branch_id
						   ,ENCRYPTBYKEY(KEY_GUID('Indigo_Symmetric_Key'),CONVERT(VARCHAR,cardsList.card_number))
						   ,cardsList.card_sequence
						   ,[dbo].[MAC](RIGHT(cardsList.card_number, 4), @objid), 1, 1);


			--Insert into load_batch_cards, links cards to load batch
			INSERT INTO load_batch_cards
				([load_batch_id], [card_id], [load_card_status_id])
			SELECT @load_batch_id, cards.card_id, @load_card_status_id
			FROM cards 
			WHERE DECRYPTBYKEY(cards.card_number) IN (SELECT cards_list.card_number
													  FROM @card_list cards_list
													  WHERE cards_list.branch_id = cards.branch_id)

			CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;--Closes sym key
					
			--declare @audit_msg nvarchar(50)
			--SET @audit_msg = 'INSERT :' + CAST(@load_batch_reference AS varchar(max))  +', LOADED'
			----log the audit record		
			--EXEC sp_insert_audit @user_id, 
			--						5, --LoadBatch
			--						NULL, 
			--						'LOADED', 
			--						@audit_msg, 
			--						NULL, NULL, NULL, NULL		

		COMMIT TRANSACTION [CREATE_LOAD_BATCH_TRAN]

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_LOAD_BATCH_TRAN]
		RETURN ERROR_MESSAGE()
	END CATCH 

	RETURN '0'
END







