-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Manually create a new distribution batch. 
-- This distributin batch is created from cards received at a card centre
-- =============================================

CREATE PROCEDURE [dbo].[usp_create_dist_batch] 
	@issuer_id int,
	@branch_id int,	
	@to_branch_id int,
	@card_issue_method_id int,
	@product_id int,
	@batch_card_size int = NULL,
	@create_batch_option int,
	@start_ref varchar(100),
	@end_ref varchar(100),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@existing_dist_batch_id bigint =null ,
	
	@ResultCode int OUTPUT,
	@dist_batchid int OUTPUT,
	@dist_batch_refnumber varchar(50) OUTPUT
AS
BEGIN

	BEGIN TRANSACTION [CREATE_DIST_BATCH]
		BEGIN TRY 

			DECLARE @number_of_dist_cards int = 0,
			@order_by_card_id bit ,
					@start_card_id bigint,
					@end_card_id bigint,
					@cards_total int = 0,
					@dist_batch_id int,
					@dist_batch_statuses_id int,
					@status_date datetimeoffset = SYSDATETIMEOFFSET(),
					@audit_msg varchar,
					@card_centre bit,
					@to_card_centre bit

			DECLARE @cards_in_batch TABLE (card_id bigint);
			SELECT @order_by_card_id = distbatch_create_order from config 
			--Determin direction of batch
			SELECT @card_centre = case when  branch_type_id =0 then 1 else 0 end
			FROM [branch]
			WHERE branch_id  = @branch_id

			SELECT @to_card_centre = case when  branch_type_id =0 then 1 else 0 end
				FROM [branch]
				WHERE branch_id  = @to_branch_id

			DECLARE @temp_cards as table(card_id bigint,card_request_reference varchar(50))

			IF(@create_batch_option = 2)
			BEGIN

				IF(@order_by_card_id = 1) -- Do batch by card_id
				BEGIN
					--Get the start card id
					SELECT @start_card_id = card_id 
					FROM [cards]				
					WHERE [cards].card_request_reference  = @start_ref
							AND [cards].product_id = @product_id						
							AND [cards].card_issue_method_id = @card_issue_method_id
							AND [cards].branch_id = @branch_id
			
					--Get the end card if
					SELECT @end_card_id = card_id 
					FROM [cards]				
					WHERE [cards].card_request_reference  = @end_ref
							AND [cards].product_id = @product_id						
							AND [cards].card_issue_method_id = @card_issue_method_id
							AND [cards].branch_id = @branch_id
		

					--Validations
					--Make sure the cards references are correct
					IF(@start_card_id IS NULL OR @end_card_id IS NULL)
					BEGIN
						SET @ResultCode = 4
						SET @dist_batchid=0
						SET @dist_batch_refnumber=0
						ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
						RETURN;
					END
					--TODO make sure start ref is smaller than end ref
					IF(@start_card_id > @end_card_id)
					BEGIN
						SET @ResultCode = 1
						SET @dist_batchid=0
						SET @dist_batch_refnumber=0
						ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
						RETURN;
					END

					
					IF(@card_centre = 1) -- Load temp cards for Card Centre
					BEGIN
						INSERT INTO @temp_cards
						SELECT [cards].card_id,[cards].card_request_reference
						FROM [cards]
								INNER JOIN [avail_cc_and_load_cards]
									ON [cards].card_id = [avail_cc_and_load_cards].card_id						
						WHERE [cards].branch_id = @branch_id
								AND [cards].product_id = @product_id							
								AND [cards].card_issue_method_id = @card_issue_method_id
								AND [cards].card_id >= @start_card_id AND [cards].card_id <= @end_card_id	

					END
					ELSE -- Load temp card for branch
					BEGIN
						INSERT INTO @temp_cards
						SELECT [cards].card_id,[cards].card_request_reference
						FROM [cards]
								INNER JOIN [branch_card_status_current]
									ON [cards].card_id = [branch_card_status_current].card_id
						WHERE [cards].branch_id = @branch_id
								AND [cards].product_id = @product_id							
								AND [branch_card_status_current].branch_card_statuses_id = 0
								AND [cards].card_issue_method_id = @card_issue_method_id
								AND [cards].card_id >= @start_card_id AND [cards].card_id <= @end_card_id	
					END
				END				
				ELSE -- Do batch 2 by ordered reference number
				BEGIN

					IF(@card_centre = 1) -- Load temp cards for Card Centre
					BEGIN
						INSERT INTO @temp_cards
						SELECT [cards].card_id,[cards].card_request_reference
						FROM [cards]
								INNER JOIN [avail_cc_and_load_cards]
									ON [cards].card_id = [avail_cc_and_load_cards].card_id						
						WHERE [cards].branch_id = @branch_id
								AND [cards].product_id = @product_id							
								AND [cards].card_issue_method_id = @card_issue_method_id
								AND ([cards].card_request_reference >= @start_ref AND [cards].card_request_reference <= @end_ref)	
								GROUP BY	[cards].card_request_reference	, [cards].card_id						
								ORDER BY [cards].card_request_reference	, [cards].card_id	

					END
					ELSE -- Load temp card for branch
					BEGIN

						INSERT INTO @temp_cards
						SELECT [cards].card_id,[cards].card_request_reference
						FROM [cards]
								INNER JOIN [branch_card_status_current]
									ON [cards].card_id = [branch_card_status_current].card_id
						WHERE [cards].branch_id = @branch_id
								AND [cards].product_id = @product_id
								--AND ((@sub_product_id IS NULL) OR ([cards].sub_product_id = @sub_product_id))
								AND [branch_card_status_current].branch_card_statuses_id = 0
								AND [cards].card_issue_method_id = @card_issue_method_id
								AND ([cards].card_request_reference >= @start_ref AND [cards].card_request_reference <= @end_ref)
								GROUP BY	[cards].card_request_reference	, [cards].card_id						
								ORDER BY [cards].card_request_reference	, [cards].card_id

					END
				END	
				
				-- Get the batch size
				SELECT @batch_card_size = COUNT(card_id)
				FROM @temp_cards					
			END

			IF(@batch_card_size = 0)
				BEGIN
					SET @ResultCode = 1
					SET @dist_batchid=0
					set @dist_batch_refnumber=0
					ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
					RETURN;
				END	


			--create the distribution batch
			INSERT INTO [dist_batch]
				([branch_id], [no_cards],[date_created],[dist_batch_reference], [card_issue_method_id],
					[dist_batch_type_id], [issuer_id], [origin_branch_id])
			VALUES (@to_branch_id, 0, @status_date, @status_date, @card_issue_method_id, 1, @issuer_id, @branch_id)

			SET @dist_batch_id = SCOPE_IDENTITY();



			IF(@card_centre = 1 AND @existing_dist_batch_id IS NULL)
			BEGIN
				--Add cards to distribution batch from card centre
				INSERT INTO [dist_batch_cards] ([dist_batch_id],[card_id],[dist_card_status_id])
				OUTPUT Inserted.card_id INTO @cards_in_batch
				SELECT TOP(@batch_card_size) @dist_batch_id, [cards].card_id, 0
				FROM [cards]
						INNER JOIN [avail_cc_and_load_cards]
							ON [cards].card_id = [avail_cc_and_load_cards].card_id						
				WHERE [cards].branch_id = @branch_id
						AND [cards].product_id = @product_id						
						AND [cards].card_issue_method_id = @card_issue_method_id
						AND (((@create_batch_option = 2) AND ([cards].card_id IN (SELECT card_id FROM @temp_cards)))
								OR @create_batch_option = 1)
				ORDER BY [cards].card_id	
				
				--remove from dist batch
				UPDATE [dist_batch_cards]
				SET [dist_batch_cards].dist_card_status_id = 0
				FROM [dist_batch_cards]
					INNER JOIN [cards]
						ON [cards].card_id = [dist_batch_cards].card_id
							AND [dist_batch_cards].dist_card_status_id = 18
					INNER JOIN [dist_batch_cards] batch_cards
						ON [cards].card_id = batch_cards.card_id
				WHERE batch_cards.dist_batch_id = @dist_batch_id

				--remove from load batch
				UPDATE [load_batch_cards]
				SET [load_batch_cards].load_card_status_id = 2
				FROM [load_batch_cards]
					INNER JOIN [cards]
						ON [cards].card_id = [load_batch_cards].card_id
							AND [load_batch_cards].load_card_status_id = 1
					INNER JOIN [dist_batch_cards]
						ON [dist_batch_cards].card_id = [cards].card_id
				WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id		
				print 2	

			END
			ELSE
			BEGIN
				IF(@existing_dist_batch_id IS NULL)
				BEGIN
					--Add cards to distribution batch from branch
					INSERT INTO [dist_batch_cards] ([dist_batch_id],[card_id],[dist_card_status_id])
					OUTPUT Inserted.card_id INTO @cards_in_batch
					SELECT TOP(@batch_card_size)
							@dist_batch_id, 
							[cards].card_id, 
							0
					FROM [cards]
							INNER JOIN [branch_card_status_current]
								ON [cards].card_id = [branch_card_status_current].card_id
					WHERE [cards].branch_id = @branch_id
							AND [cards].product_id = @product_id								
							AND [branch_card_status_current].branch_card_statuses_id = 0
							AND [cards].card_issue_method_id = @card_issue_method_id
							AND (((@create_batch_option = 2) AND ([cards].card_id IN (SELECT card_id FROM @temp_cards)))
									OR @create_batch_option = 1)
					ORDER BY [cards].card_id

					--remove from dist batch
					UPDATE t
					SET t.dist_card_status_id = 22
					FROM [dist_batch_cards] t
						INNER JOIN [dist_batch_cards] s
							ON t.card_id = s.card_id
					WHERE t.dist_card_status_id IN (2, 7)
						AND s.dist_batch_id = @dist_batch_id

				END 
				ELSE IF(@existing_dist_batch_id IS NOT NULL)
				BEGIN
					-- Link cards to the new dist batch from the old dist_batch
					IF(@card_centre = 0)
					BEGIN
						--Available cards for distribution at a branch
						INSERT INTO [dist_batch_cards] ([dist_batch_id], [card_id], [dist_card_status_id])
						OUTPUT Inserted.card_id INTO @cards_in_batch
						SELECT TOP(@batch_card_size) @dist_batch_id, [dist_batch_cards].card_id, 0
						FROM [dbo].[dist_batch]
								INNER JOIN [dbo].[dist_batch_status_current] 
									ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
								INNER JOIN [dbo].[dist_batch_cards] 
									ON [dist_batch_cards].[dist_batch_id] = [dist_batch].[dist_batch_id]
								INNER JOIN [branch]
									ON ([dist_batch_status_current].dist_batch_statuses_id != 8 AND [branch].branch_id = [dist_batch].branch_id)
										OR
									([dist_batch_status_current].dist_batch_statuses_id = 8 AND [branch].branch_id = [dist_batch].origin_branch_id)
								INNER JOIN [dbo].[branch_card_status_current] 
									ON [branch_card_status_current].[card_id] = [dist_batch_cards].[card_id]
										AND [branch_card_status_current].[branch_id] = [branch].[branch_id]
								INNER JOIN [cards]
									ON [cards].card_id = [dist_batch_cards].card_id
						WHERE [dist_batch_status_current].dist_batch_statuses_id IN (3, 8)
								AND [dist_batch_cards].[dist_card_status_id] IN (2, 7)
								AND [branch_card_status_current].[branch_card_statuses_id] = 0
								AND [dist_batch].[dist_batch_id] = @existing_dist_batch_id
								AND [cards].product_id = @product_id

					END
					ELSE
					BEGIN
						--Available cards for distribution at card center
						INSERT INTO [dist_batch_cards] ([dist_batch_id], [card_id], [dist_card_status_id])
						OUTPUT Inserted.card_id INTO @cards_in_batch
						SELECT TOP(@batch_card_size) @dist_batch_id, [dist_batch_cards].card_id, 0
								FROM [dbo].[dist_batch]
								INNER JOIN [dbo].[dist_batch_status_current] 
									ON [dist_batch_status_current].dist_batch_id = [dist_batch].dist_batch_id
								INNER JOIN [dbo].[dist_batch_cards] 
									ON [dist_batch_cards].[dist_batch_id] = [dist_batch].[dist_batch_id]
								INNER JOIN [cards]
									ON [cards].card_id = [dist_batch_cards].card_id
						WHERE [dist_batch_status_current].dist_batch_statuses_id = 14
								AND [dist_batch_cards].[dist_card_status_id] = 18
								AND [dist_batch].[dist_batch_id] = @existing_dist_batch_id
								AND [cards].product_id = @product_id
					END

					--remove from dist batch
					UPDATE t 
						SET [dist_card_status_id] = 22 
					FROM [dist_batch_cards] t 
							INNER JOIN [dist_batch_cards] s	ON t.card_id = s.card_id
						WHERE t.dist_batch_id = @existing_dist_batch_id
							AND s.dist_batch_id = @dist_batch_id	
										
				END
					-- Card are at a branch and must have their status changed

					UPDATE t
					SET t.branch_id = @branch_id, 
						t.branch_card_statuses_id = 13, 
						t.status_date = SYSDATETIMEOFFSET(), 
						t.[user_id] = @audit_user_id, 
						t.operator_user_id = NULL,
						t.branch_card_code_id = NULL,
						t.comments = NULL,
						t.pin_auth_user_id = NULL
					OUTPUT Deleted.* INTO branch_card_status_audit
					FROM branch_card_status t INNER JOIN [dist_batch_cards] s
							ON t.card_id = s.card_id
					WHERE s.dist_batch_id = @dist_batch_id

					--INSERT INTO [branch_card_status] (card_id,branch_id, branch_card_statuses_id, comments, status_date, [user_id])
					--SELECT card_id,@branch_id, 13, '', GETDATE(), @audit_user_id
					--FROM [dist_batch_cards]
					--WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id	
			END
							
			--Get the number of cards inserted
			SELECT @number_of_dist_cards = COUNT(card_id) FROM @cards_in_batch											
			
			--Make sure we've insered enough cards
			IF(@number_of_dist_cards = @batch_card_size)
			BEGIN
				IF(@card_centre = 1)
				BEGIN
					SET @dist_batch_statuses_id = 0
					
					--add dist batch status of created
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@dist_batch_id, @dist_batch_statuses_id, @audit_user_id, @status_date, 'Batch Created')
				END
				ELSE
				BEGIN

				
					if(@to_card_centre=1)
					SET @dist_batch_statuses_id = 19
					else
					SET @dist_batch_statuses_id = 2

					--add dist batch status of created
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@dist_batch_id, @dist_batch_statuses_id, @audit_user_id, @status_date, 'Batch Created')
				END

				--Generate dist batch reference
				DECLARE @dist_batch_ref varchar(50)
				DECLARE @dist_version int = 0

				IF(@existing_dist_batch_id IS NOT NULL) -- Reference based on previous ref number
					BEGIN
						DECLARE @existing_dist_batch_ref varchar(50)
						DECLARE @current_version int = 0

						SELECT @existing_dist_batch_ref = [dist_batch_reference], @current_version = [dist_version]
						FROM dist_batch 
						WHERE dist_batch_id = @existing_dist_batch_id

						IF(@current_version > 0)
							SET @existing_dist_batch_ref = LEFT(@existing_dist_batch_ref, LEN(@existing_dist_batch_ref) - LEN('_' + CAST(@current_version as NVARCHAR(10))))
						
						-- Get the current number of dist batch version
						SELECT @dist_version = COUNT(dist_batch_id)
						FROM [dist_batch]
						WHERE [dist_batch_reference] LIKE (@existing_dist_batch_ref + '%')

						SET @dist_batch_ref = @existing_dist_batch_ref + '_' + cast(@dist_version as nvarchar(10))			
					END
				ELSE --Build the normal reference number
					SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
							[branch].branch_code + '' + 
							CONVERT(VARCHAR(8), @status_date, 112) + '' +
							CAST(@dist_batch_id AS varchar(max))
					FROM [branch] INNER JOIN [issuer]
						ON [branch].issuer_id = [issuer].issuer_id
					WHERE [branch].branch_id = @branch_id
				
				
				--UPDATE dist batch with reference, version and number of cards
				UPDATE [dist_batch]
				SET [dist_batch_reference] = @dist_batch_ref,
					[no_cards] = @number_of_dist_cards,
					[dist_version] = @dist_version
				WHERE [dist_batch].dist_batch_id = @dist_batch_id
					

				----UPDATE the load batch cards status to allocated							
				--UPDATE [load_batch_cards]
				--SET [load_batch_cards].load_card_status_id = 2
				--WHERE [load_batch_cards].card_id IN 
				--		(SELECT [dist_batch_cards].card_id
				--		 FROM [dist_batch_cards]
				--		 WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id)

				--IF(@card_centre = 1 AND @existing_dist_batch_id IS NULL)
				--BEGIN
				
				--	UPDATE [dist_batch_cards]
				--	SET [dist_batch_cards].dist_card_status_id = 0
				--	FROM [dist_batch_cards]
				--		INNER JOIN [cards]
				--			ON [cards].card_id = [dist_batch_cards].card_id
				--				AND [dist_batch_cards].dist_card_status_id = 18
				--		INNER JOIN [dist_batch_cards] batch_cards
				--			ON [cards].card_id = batch_cards.card_id
				--	WHERE batch_cards.dist_batch_id = @dist_batch_id

				--	UPDATE [load_batch_cards]
				--	SET [load_batch_cards].load_card_status_id = 2
				--	FROM [load_batch_cards]
				--		INNER JOIN [cards]
				--			ON [cards].card_id = [load_batch_cards].card_id
				--				AND [load_batch_cards].load_card_status_id = 1
				--		INNER JOIN [dist_batch_cards]
				--			ON [dist_batch_cards].card_id = [cards].card_id
				--	WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id	

				--END
				--ELSE
				--BEGIN
				--	UPDATE t
				--	SET t.branch_id = @branch_id, 
				--		t.branch_card_statuses_id = 13, 
				--		t.status_date = SYSDATETIMEOFFSET(), 
				--		t.[user_id] = @audit_user_id, 
				--		t.operator_user_id = NULL,
				--		t.branch_card_code_id = NULL,
				--		t.comments = NULL,
				--		t.pin_auth_user_id = NULL
				--	OUTPUT Deleted.* INTO branch_card_status_audit
				--	FROM branch_card_status t INNER JOIN [dist_batch_cards] s
				--			ON t.card_id = s.card_id
				--	WHERE s.dist_batch_id = @dist_batch_id

					--INSERT INTO [branch_card_status] (card_id,branch_id, branch_card_statuses_id, comments, status_date, [user_id])
					--SELECT card_id,@branch_id, 13, '', SYSDATETIMEOFFSET(), @audit_user_id
					--FROM [dist_batch_cards]
					--WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
				--END

				--Update the cards to the new destination branch.
				UPDATE [cards]
				SET branch_id = @to_branch_id
				FROM [cards]
						INNER JOIN [dist_batch_cards]
							ON [cards].card_id = [dist_batch_cards].card_id
				WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id

				----- if the existing batch is moved all cards from that batch. the batch should updated to removed.
				--if(@existing_dist_batch_id is not null)---inserting cards from another dist batch.
				--	BEGIN 

				--	Declare @active_cards_count int;

				--	SELECT  @active_cards_count=COUNT(*) from dist_batch_cards 		
				--	WHERE [dist_batch_cards].dist_batch_id = @existing_dist_batch_id
				--	and [dist_batch_cards].dist_card_status_id<>22

				--	if(@active_cards_count=0)
				--		INSERT INTO [dbo].[dist_batch_status]
				--		([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				--	VALUES(@existing_dist_batch_id, 22, @audit_user_id, @status_date, 'Batch Removed Created.')
 
				--	END

				--NOTIFICATION of BATCH
				EXEC usp_notification_batch_add @dist_batch_id, @dist_batch_statuses_id	
				
				DECLARE @dist_batch_status_name varchar(50)
				SELECT @dist_batch_status_name =  dist_batch_status_name
				FROM dist_batch_statuses
				WHERE dist_batch_statuses_id = 0

				--Add audit for dist batch creation								
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
				SET @dist_batchid=@dist_batch_id
				SET @dist_batch_refnumber=@dist_batch_ref

				COMMIT TRANSACTION [CREATE_DIST_BATCH]	
				print @ResultCode
			END
			ELSE
			BEGIN
				--Size fo cards for batch doesnt match number of records inserted.
				SET @ResultCode = 70
				SET @dist_batchid=0
				SET @dist_batch_refnumber=0
				ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
			END						
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_DIST_BATCH]
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

