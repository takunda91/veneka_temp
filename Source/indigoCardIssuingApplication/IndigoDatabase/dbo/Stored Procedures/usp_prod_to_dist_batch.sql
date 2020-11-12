-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_prod_to_dist_batch] 
	-- Add the parameters for the stored procedure here
	@dist_batch_id bigint, 
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@autogenerate_dist_batch_id bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	--BEGIN TRANSACTION [PROD_TO_DIST_TRAN]
	--BEGIN TRY 

		--Get a distinct list of branches from the batch
		DECLARE @branch_id int,
				@cards_total int = 0,
				@card_issue_method_id int,
				@check_prod_batch_id int,
				@new_dist_batch_id int,
				@cc_branch_id int,
				@audit_msg varchar(max),
				@proddist_Batch_ref nvarchar(max)

		SELECT @cc_branch_id = [branch].branch_id
		FROM [branch] INNER JOIN [dist_batch]
			ON [branch].issuer_id = [dist_batch].issuer_id
				AND [branch].branch_type_id = 0 
				AND [branch].branch_status_id = 0		 

		SELECT @card_issue_method_id = card_issue_method_id, @check_prod_batch_id = dist_batch_type_id,@proddist_Batch_ref=dist_batch_reference
		FROM dist_batch
		WHERE dist_batch_id = @dist_batch_id

		IF (@check_prod_batch_id = 1)
			RAISERROR ('Can only create distribution batchs from a production batch.', 12, 12 );


		--Loop through all distinct branches for the production batch and create distribution batches
		DECLARE branchId_cursor CURSOR FOR 
			SELECT DISTINCT [cards].delivery_branch_id
			FROM [cards] 
				INNER JOIN [dist_batch_cards]
					ON [cards].card_id = [dist_batch_cards].card_id
			WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
						

		OPEN branchId_cursor

		FETCH NEXT FROM branchId_cursor 
		INTO @branch_id

		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @number_of_dist_cards int = 0,
					@dist_status_date datetimeoffset = SYSDATETIMEOFFSET()

			--IF(SELECT [issuer].auto_create_dist_batch
			--	FROM [issuer] INNER JOIN [branch]
			--			ON [issuer].issuer_id = [branch].issuer_id
			--	WHERE [branch].branch_id = @branch_id) = 1
			--	BEGIN							
								
					--create the distribution batch
					INSERT INTO [dist_batch]
						([issuer_id], [branch_id], [no_cards],[date_created],[dist_batch_reference], [card_issue_method_id], [dist_batch_type_id])
					SELECT issuer_id, @branch_id, 0, @dist_status_date, @dist_status_date, @card_issue_method_id, 1
					FROM [branch]
					WHERE branch_id = @branch_id

					SET @new_dist_batch_id = SCOPE_IDENTITY();

					--Add cards to distribution batch
					INSERT INTO [dist_batch_cards]
						([dist_batch_id],[card_id],[dist_card_status_id])
					SELECT
						@new_dist_batch_id, [cards].card_id, 0
					FROM [cards] INNER JOIN [dist_batch_cards]
						ON [cards].card_id = [dist_batch_cards].card_id
					WHERE [dist_batch_cards].dist_batch_id = @dist_batch_id
							AND [cards].delivery_branch_id = @branch_id
							
					--Get the number of cards inserted
					SELECT @number_of_dist_cards = @@ROWCOUNT										

					--add dist batch status of created
					INSERT INTO [dbo].[dist_batch_status]
						([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					VALUES(@new_dist_batch_id, 0, @audit_user_id, @dist_status_date, 'Distribution Batch Create From Production: ' + CONVERT(VARCHAR(max),@dist_batch_id))

					--Generate dist batch reference
					DECLARE @dist_batch_ref varchar(50)
					SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
												[branch].branch_code + '' + 
												CONVERT(VARCHAR(8), @dist_status_date, 112) + '' +
												CAST(@new_dist_batch_id AS varchar(max))
					FROM [branch] INNER JOIN [issuer]
						ON [branch].issuer_id = [issuer].issuer_id
					WHERE [branch].branch_id = @branch_id

				
					--- if auto generate flag is false generate new distbatchref with branchcode+'_'+productionbatch reference
					if(@autogenerate_dist_batch_id =0 and @card_issue_method_id=0)
					begin
					Declare @Branchcode varchar(max)
					select @Branchcode=branch.branch_code from branch where branch.branch_id=@branch_id
					set @dist_batch_ref =@Branchcode+'_'+ @proddist_Batch_ref
					END

					--UPDATE dist batch with reference and number of cards
					UPDATE [dist_batch]
					SET [dist_batch_reference] = @dist_batch_ref,
						[no_cards] = @number_of_dist_cards
					WHERE [dist_batch].dist_batch_id = @new_dist_batch_id

					--UPDATE the production batch cards status to allocated							
					UPDATE [dist_batch_cards]
					SET [dist_batch_cards].dist_card_status_id = 0
					WHERE dist_batch_id = @dist_batch_id
						AND	[dist_batch_cards].card_id IN 
							(SELECT [dist_batch_cards].card_id
								FROM [dist_batch_cards]
								WHERE [dist_batch_cards].dist_batch_id = @new_dist_batch_id)

					--UPDATE cards to delivery branch ID
					UPDATE [cards]
					SET branch_id = @branch_id,
					    origin_branch_id = COALESCE(@cc_branch_id, @branch_id)
					WHERE card_id IN (SELECT [dist_batch_cards].card_id
									  FROM [dist_batch_cards]
									  WHERE [dist_batch_cards].dist_batch_id = @new_dist_batch_id)
							
					--NOTIFICATION
					EXEC usp_notification_batch_add @new_dist_batch_id, 0

					DECLARE @dist_batch_status_name varchar(50)
					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = 0

					--Add audit for dist batch creation								
					SET @audit_msg = 'Create: ' + CAST(@new_dist_batch_id AS varchar(max)) +
										', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					--TODO: look at the dist_batch_flow table
					--auto add approve the distbatch
					UPDATE dist_batch_status 
					SET [dist_batch_statuses_id] = 1, 
						[user_id] = @audit_user_id, 
						[status_date] = DATEADD(ss,1,@dist_status_date), 
						[status_notes] = 'Auto Dist Batch Create Approval'	
					OUTPUT Deleted.* INTO dist_batch_status_audit
					WHERE [dist_batch_id] = @new_dist_batch_id

					--INSERT INTO [dbo].[dist_batch_status]
					--	([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
					--VALUES(@new_dist_batch_id, 1, @audit_user_id, DATEADD(ss,1,@dist_status_date), 'Auto Dist Batch Create Approval')								

					SELECT @dist_batch_status_name =  dist_batch_status_name
					FROM dist_batch_statuses
					WHERE dist_batch_statuses_id = 1

					--Add audit for dist batch update								
					SET @audit_msg = 'Update: ' + CAST(@new_dist_batch_id AS varchar(max)) +
										', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

			--	END
			--ELSE
			--	BEGIN
			--		--Update the cards linked to the load batch and cursors branch with the available status.
			--		UPDATE [load_batch_cards]
			--		SET load_card_status_id = 1
			--		FROM [load_batch_cards] INNER JOIN [cards]
			--				ON [load_batch_cards].card_id = [cards].card_id
			--		WHERE [load_batch_cards].load_batch_id = @load_batch_id
			--				AND [cards].branch_id = @branch_id

			--		--Get the number of cards updated
			--		SELECT @number_of_dist_cards = @@ROWCOUNT
			--	END						

			SELECT @cards_total += @number_of_dist_cards

				-- Get the next branch.
			FETCH NEXT FROM branchId_cursor 
			INTO @branch_id
			END 
		CLOSE branchId_cursor;
		DEALLOCATE branchId_cursor;

		--Check that all cards for the load batch have been updated
		IF (SELECT COUNT(card_id) FROM [dist_batch_cards] WHERE dist_batch_id = @dist_batch_id) != @cards_total
		BEGIN
			RAISERROR ('Not all cards have been moved from production batch to distribution batch.',
						12,
						12 );
		END

	--	COMMIT TRANSACTION [PROD_TO_DIST_TRAN]				
	--	END TRY
	--BEGIN CATCH
	--	ROLLBACK TRANSACTION [PROD_TO_DIST_TRAN]
	--	DECLARE @ErrorMessage NVARCHAR(4000);
	--	DECLARE @ErrorSeverity INT;
	--	DECLARE @ErrorState INT;

	--	SELECT 
	--		@ErrorMessage = ERROR_MESSAGE(),
	--		@ErrorSeverity = ERROR_SEVERITY(),
	--		@ErrorState = ERROR_STATE();

	--	RAISERROR (@ErrorMessage, -- Message text.
	--			   @ErrorSeverity, -- Severity.
	--			   @ErrorState -- State.
	--			   );
	--END CATCH


END





