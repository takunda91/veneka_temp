
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_pin_prod_to_pin_batch] 
	-- Add the parameters for the stored procedure here
	@pin_batch_id bigint, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN

		--Get a pininct list of branches from the batch
		DECLARE @branch_id int,
				@cards_total int = 0,
				@card_issue_method_id int,
				@check_prod_batch_id int,
				@new_pin_batch_id int,
				@audit_msg varchar(max)

		SELECT @card_issue_method_id = card_issue_method_id, @check_prod_batch_id = pin_batch_type_id
		FROM pin_batch
		WHERE pin_batch_id = @pin_batch_id

		IF (@check_prod_batch_id = 1)
			RAISERROR ('Can only create pin distribution batchs from a production batch.', 12, 12 );


		--Loop through all pininct branches for the production batch and create pinribution batches
		DECLARE branchId_cursor CURSOR FOR 
			SELECT DISTINCT [cards].branch_id
			FROM [cards] 
				INNER JOIN [pin_batch_cards]
					ON [cards].card_id = [pin_batch_cards].card_id
			WHERE [pin_batch_cards].pin_batch_id = @pin_batch_id
						

		OPEN branchId_cursor

		FETCH NEXT FROM branchId_cursor 
		INTO @branch_id

		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @number_of_pin_cards int = 0,
					@pin_status_date datetimeoffset = SYSDATETIMEOFFSET()						
								
					--create the batch
					INSERT INTO [pin_batch]
						([issuer_id], [branch_id], [no_cards],[date_created],[pin_batch_reference], [card_issue_method_id], [pin_batch_type_id])
					SELECT issuer_id, @branch_id, 0, @pin_status_date, @pin_status_date, @card_issue_method_id, 1
					FROM [branch]
					WHERE branch_id = @branch_id

					SET @new_pin_batch_id = SCOPE_IDENTITY();

					--Add cards to pinribution batch
					INSERT INTO [pin_batch_cards]
						([pin_batch_id],[card_id],[pin_batch_cards_statuses_id])
					SELECT
						@new_pin_batch_id, [cards].card_id, 0
					FROM [cards] INNER JOIN [pin_batch_cards]
						ON [cards].card_id = [pin_batch_cards].card_id
					WHERE [pin_batch_cards].pin_batch_id = @pin_batch_id
							AND [cards].branch_id = @branch_id
							
					--Get the number of cards inserted
					SELECT @number_of_pin_cards = @@ROWCOUNT										

					--add pin batch status of created
					INSERT INTO [dbo].[pin_batch_status]
						([pin_batch_id],[pin_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@new_pin_batch_id, 0, @audit_user_id, @pin_status_date, 'PIN Distribution Batch Create From: ' + CONVERT(VARCHAR(max),@pin_batch_id))

					--Generate pin batch reference
					DECLARE @pin_batch_ref varchar(50)
					SELECT @pin_batch_ref =  [issuer].issuer_code + '' + 
												[branch].branch_code + '' + 
												CONVERT(VARCHAR(8), @pin_status_date, 112) + '' +
												CAST(@new_pin_batch_id AS varchar(max))
					FROM [branch] INNER JOIN [issuer]
						ON [branch].issuer_id = [issuer].issuer_id
					WHERE [branch].branch_id = @branch_id

					--UPDATE pin batch with reference and number of cards
					UPDATE [pin_batch]
					SET [pin_batch_reference] = @pin_batch_ref,
						[no_cards] = @number_of_pin_cards
					WHERE [pin_batch].pin_batch_id = @new_pin_batch_id

					--UPDATE the production batch cards status to allocated							
					UPDATE [pin_batch_cards]
					SET [pin_batch_cards].pin_batch_cards_statuses_id = 1
					WHERE pin_batch_id = @pin_batch_id
						AND	[pin_batch_cards].card_id IN 
							(SELECT [pin_batch_cards].card_id
								FROM [pin_batch_cards]
								WHERE [pin_batch_cards].pin_batch_id = @new_pin_batch_id)
							

					DECLARE @pin_batch_status_name varchar(50)
					SELECT @pin_batch_status_name =  pin_batch_statuses_name
					FROM pin_batch_statuses
					WHERE pin_batch_statuses_id = 0

					--Add audit for pin batch creation								
					SET @audit_msg = 'Create: ' + CAST(@new_pin_batch_id AS varchar(max)) +
										', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					
					----auto add approve the pinbatch
					--INSERT INTO [dbo].[pin_batch_status]
					--	([pin_batch_id],[pin_batch_statuses_id],[user_id],[status_date],[status_notes])
					--VALUES(@new_pin_batch_id, 1, @audit_user_id, DATEADD(ss,1,@pin_status_date), 'Auto pin Batch Create Approval')								

					--SELECT @pin_batch_status_name =  pin_batch_statuses_name
					--FROM pin_batch_statuses
					--WHERE pin_batch_statuses_id = 1

					----Add audit for pin batch update								
					--SET @audit_msg = 'Update: ' + CAST(@new_pin_batch_id AS varchar(max)) +
					--					', ' + COALESCE(@pin_batch_ref, 'UNKNOWN') +
					--					', ' + COALESCE(@pin_batch_status_name, 'UNKNOWN')
								   
					----log the audit record		
					--EXEC usp_insert_audit @audit_user_id, 
					--						2,
					--						NULL, 
					--						@audit_workstation, 
					--						@audit_msg, 
					--						NULL, NULL, NULL, NULL

				

			SELECT @cards_total += @number_of_pin_cards

				-- Get the next branch.
			FETCH NEXT FROM branchId_cursor 
			INTO @branch_id
			END 
		CLOSE branchId_cursor;
		DEALLOCATE branchId_cursor;

		--Check that all cards for the load batch have been updated
		IF (SELECT COUNT(card_id) FROM [pin_batch_cards] WHERE pin_batch_id = @pin_batch_id) != @cards_total
		BEGIN
			RAISERROR ('Not all cards have been moved from production batch to pinribution batch.',
						12,
						12 );
		END
END