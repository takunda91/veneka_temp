CREATE  PROCEDURE [dbo].[usp_request_hybrid_create_print_batch] 
	@card_issue_method_id int,
	@issuer_id int,
	@branch_id int = null,
	@product_id int,
	@card_priority_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@requests_in_batch int OUTPUT,
	@print_batch_id int OUTPUT,
	@print_batch_ref varchar(50) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [CREATE_DIST_BATCH]
		BEGIN TRY 

		SET @print_batch_id = 0
		SET @print_batch_ref = ''
		set @requests_in_batch=0

		DECLARE @hybrid_request_statuses_id int
		DECLARE @main_branch_id int
		SET @hybrid_request_statuses_id =1

		SELECT @main_branch_id = main_branch_id
			FROM [branch]
			WHERE branch_id  = @branch_id

		DECLARE @stock_cardcount INT, @requests_tobein_batch INT
	
		EXEC   [usp_get_mainbranch_card_count] @issuer_id,@branch_id,@product_id,@card_issue_method_id,@audit_user_id,@audit_workstation, @stock_cardcount output
		--select @stock_cardcount
		 

		SELECT @requests_tobein_batch=COUNT(*) 
					 FROM hybrid_request_status_current
							INNER JOIN branch
								ON hybrid_request_status_current.branch_id = branch.branch_id										
					 WHERE hybrid_request_statuses_id = @hybrid_request_statuses_id
							AND product_id = @product_id
							AND card_issue_method_id = @card_issue_method_id
							AND card_priority_id = @card_priority_id
							AND hybrid_request_status_current.delivery_branch_id = @branch_id
							AND issuer_id = @issuer_id
--print @requests_tobein_batch
		--Only create a batch if there are cards for the batch
		IF(  @requests_tobein_batch = 0)
		BEGIN
			set @print_batch_id = 0
			SET @ResultCode = 400
			COMMIT TRANSACTION [CREATE_DIST_BATCH]
		END	
		ELSE  IF(@card_issue_method_id=1 and  @stock_cardcount<@requests_tobein_batch)
		BEGIN
			set @print_batch_id = 0
			SET @ResultCode = 808
			COMMIT TRANSACTION [CREATE_DIST_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@batch_branch_id int,
						@audit_msg nvarchar(500)


			

				--create the print batch
				INSERT INTO [print_batch]
					([card_issue_method_id],[issuer_id],no_of_requests,[branch_id],[date_created],[print_batch_reference])
				VALUES (@card_issue_method_id, @issuer_id,0, @branch_id, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())

				SET @print_batch_id = SCOPE_IDENTITY();

			
				IF( @card_issue_method_id =1)
				BEGIN
				--Add cards to production batch
				INSERT INTO [print_batch_requests]
					([print_batch_id],[request_id])
				SELECT @print_batch_id, request_id
				FROM hybrid_request_status_current
							INNER JOIN branch
								ON hybrid_request_status_current.branch_id = branch.branch_id										
					 WHERE hybrid_request_statuses_id = @hybrid_request_statuses_id
							AND product_id = @product_id
							AND card_issue_method_id = @card_issue_method_id
							AND card_priority_id = @card_priority_id
							AND hybrid_request_status_current.delivery_branch_id = COALESCE(@branch_id, hybrid_request_status_current.delivery_branch_id)
							AND issuer_id = @issuer_id

					--INSERT INTO [dist_batch_cards_temp]
					--([dist_batch_id],[card_id])
					--SELECT [dist_batch_id],[card_id] 
					--from  [dist_batch_cards]
					--where [dist_batch_cards].[dist_batch_id]=@dist_batch_id
				END
				Declare @no_of_request int
				select	 @no_of_request=  count([request_id]) from [print_batch_requests] where print_batch_id=@print_batch_id
				update [print_batch] set no_of_requests=@no_of_request where print_batch.print_batch_id=@print_batch_id

				--add prod batch status of created
				INSERT INTO [dbo].[print_batch_status]
					([print_batch_id],[print_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@print_batch_id, 0, @audit_user_id, SYSDATETIMEOFFSET(), 'Batch Created')

				--Generate dist batch reference
				SELECT @print_batch_ref =  [issuer].issuer_code + '' + 
										  CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										  CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + '' +
										  CAST(@print_batch_id AS varchar(max))
				FROM [issuer]					
					INNER JOIN [issuer_product]
						ON [issuer_product].issuer_id = [issuer].issuer_id
				WHERE [issuer].issuer_id = @issuer_id

				

				SELECT @requests_in_batch = COUNT(*)
				FROM print_batch_requests
				WHERE print_batch_id = @print_batch_id 

				--UPDATE prod batch with reference and number of cards
				UPDATE [print_batch]
				SET [print_batch_reference] = @print_batch_ref				
				WHERE [print_batch].print_batch_id = @print_batch_id

				--UPDATE branch card status for those cards that have been added to the new dist batch.
				UPDATE t
				SET t.branch_id = [hybrid_requests].branch_id, 
					t.hybrid_request_statuses_id = 2, 
					t.status_date = DATEADD(s, 1,SYSDATETIMEOFFSET()), 
					t.[user_id] = @audit_user_id, 
					t.operator_user_id = @audit_user_id,
					t.comments = 'Assigned to batch'
				OUTPUT Deleted.* INTO hybrid_request_status_audit
				FROM hybrid_request_status t 
						INNER JOIN [print_batch_requests] s ON t.request_id = s.request_id
						INNER JOIN [hybrid_requests] ON [hybrid_requests].request_id = s.request_id
				WHERE s.print_batch_id = @print_batch_id

				

				--Notification
				EXEC usp_notification_batch_add @print_batch_id, 0

				--Add audit for dist batch creation	
				DECLARE @print_batch_status_name varchar(50)
				SELECT @print_batch_status_name =  print_batch_statuses
				FROM print_batch_statuses
				WHERE print_batch_statuses_id = 0
											
				SET @audit_msg = 'Create: ' + CAST(@print_batch_id AS varchar(max)) +
									', ' + COALESCE(@print_batch_ref, 'UNKNOWN') +
									', ' + COALESCE(@print_batch_status_name, 'UNKNOWN')
								   
				--log the audit record		
				EXEC usp_insert_audit @audit_user_id, 
										2,
										NULL, 
										@audit_workstation, 
										@audit_msg, 
										NULL, NULL, NULL, NULL

				set @ResultCode = 0
				COMMIT TRANSACTION [CREATE_DIST_BATCH]	

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