CREATE PROCEDURE [dbo].[usp_print_batch_spoil] 
	@print_batch_id bigint,
	@new_print_batch_statuses_id int,
	@request_list [dbo].request_id_array readonly,
	@status_notes varchar(150) = null,
	@language_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [PRINT_BATCH_STATUS_CHANGE]
		BEGIN TRY 
			
			DECLARE @current_print_batch_status_id int,
					@audit_msg varchar(max),
					@original_batch_type_id int	,					
					@print_batch_statuses_id int,
					@branch_card_code_id int

					
			select @print_batch_statuses_id =print_batch_status.print_batch_statuses_id from print_batch_status where print_batch_id=print_batch_id
			--Check that someone hasn't already updated the dist batch
			IF(dbo.[PrintBatchInCorrectStatus](@print_batch_statuses_id, @new_print_batch_statuses_id, @print_batch_id) =1)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					--Update the pin batch status.
					UPDATE print_batch_status
						SET [print_batch_statuses_id] = @new_print_batch_statuses_id, 
						[user_id] = @audit_user_id, 
						[status_date] = SYSDATETIMEOFFSET(), 
						[status_notes] = @status_notes
					OUTPUT Deleted.* INTO pin_batch_status_audit
					WHERE [print_batch_id] = @print_batch_id

					---putting back the request into batch
					UPDATE t
				SET t.branch_id = [hybrid_requests].branch_id, 
					t.hybrid_request_statuses_id = 1, 
					t.status_date = DATEADD(s, 1,SYSDATETIMEOFFSET()), 
					t.[user_id] = @audit_user_id, 
					t.operator_user_id = @audit_user_id,
					t.comments = 'Putting back the request into pool'
				OUTPUT Deleted.* INTO hybrid_request_status_audit
				FROM hybrid_request_status t 
						INNER JOIN [print_batch_requests] s ON t.request_id = s.request_id
						INNER JOIN [hybrid_requests] ON [hybrid_requests].request_id = s.request_id
						inner join @request_list r on r.request_id=t.request_id
				WHERE s.print_batch_id = @print_batch_id and r.[request_statues_id]=8

			
					
			Declare @card_list as [dbo].[card_id_array]

			insert into @card_list(card_id,[branch_card_statuses_id])
			select cc.card_id,0
			from @request_list r
			LEFT join [dbo].[customer_account_requests] as cr on cr.request_id=r.request_id
				LEFT JOIN [dbo].[customer_account_cards] as cc on cc.customer_account_id=cr.customer_account_id
				where  r.request_statues_id=8

				DECLARE @id int
				DECLARE @pass varchar(100)

				DECLARE cur CURSOR FOR SELECT card_id FROM @card_list
				OPEN cur

				FETCH NEXT FROM cur INTO @id

				WHILE @@FETCH_STATUS = 0 BEGIN
					EXEC usp_issue_card_spoil @id, @branch_card_code_id,'spoiled card because failed to upload in cms.',
							 @audit_user_id,
							 @audit_workstation,
							 @ResultCode
					FETCH NEXT FROM cur INTO @id, @pass
				END

				CLOSE cur    
				DEALLOCATE cur
					
					DECLARE @print_batch_status_name varchar(50),
							@print_batch_ref varchar(100)

					SELECT @print_batch_status_name =  print_batch_statuses
					FROM print_batch_statuses
					WHERE print_batch_statuses_id = @print_batch_statuses_id

					SELECT @print_batch_ref = print_batch_reference
					FROM print_batch
					WHERE print_batch_id = @print_batch_id

					--Add audit for pin batch update								
					SET @audit_msg = 'Update: ' + CAST(@print_batch_id AS varchar(max)) +
										', ' + COALESCE(@print_batch_ref, 'UNKNOWN') +
										', ' + COALESCE(@print_batch_status_name, 'UNKNOWN')
			
								   
					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
											2,
											NULL, 
											@audit_workstation, 
											@audit_msg, 
											NULL, NULL, NULL, NULL

					SET @ResultCode = 0					
				END

				--Fetch the pin batch with latest details
				EXEC usp_get_print_batch @print_batch_id,
										@language_id,
										@audit_user_id,
										@audit_workstation

				COMMIT TRANSACTION [PRINT_BATCH_STATUS_CHANGE]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [PRINT_BATCH_STATUS_CHANGE]
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

	


