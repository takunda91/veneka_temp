-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_cms_upload_batch] 
	@card_issue_method_id int,
	@issuer_id int,
	@branch_id int = null,
	@product_id int = null,
	@card_priority_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@cards_in_batch int OUTPUT,
	@dist_batch_id bigint OUTPUT,
	--@dist_batch_ref varchar(50) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [CREATE_CMS_BATCH]
		BEGIN TRY 

		DECLARE @dist_batch_ref varchar(50)

		SET @cards_in_batch = 0
		SET	@dist_batch_id = 0
		SET @dist_batch_ref = ''

		DECLARE @branch_card_statuses_id int
		SET @branch_card_statuses_id = 6

		--Only create a batch if there are cards for the batch
		IF( (SELECT COUNT(*) 
			 FROM branch_card_status_current
					INNER JOIN branch
						ON branch_card_status_current.branch_id = branch.branch_id						
			 WHERE branch_card_statuses_id = @branch_card_statuses_id
					AND product_id = COALESCE(@product_id, product_id)
					AND card_issue_method_id = @card_issue_method_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id
					AND card_id NOT IN(SELECT card_id 
										FROM [dist_batch_cards] 
											INNER JOIN [dist_batch]
												ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
												AND [dist_batch].dist_batch_type_id = 0
												AND [dist_batch].issuer_id = @issuer_id)) = 0)
		BEGIN
			SET @ResultCode = 400
			COMMIT TRANSACTION [CREATE_CMS_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@batch_branch_id int,
						@audit_msg nvarchar(500)


				--create the production batch
				INSERT INTO [dist_batch]
					([card_issue_method_id],[issuer_id],[branch_id], [no_cards],[date_created],[dist_batch_reference],[dist_batch_type_id])
				VALUES (@card_issue_method_id, @issuer_id, @branch_id, 0, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(),0)

				SET @dist_batch_id = SCOPE_IDENTITY();

				--Add cards to production batch
				INSERT INTO [dist_batch_cards]
					([dist_batch_id],[card_id],[dist_card_status_id])
				SELECT @dist_batch_id, card_id, 12
				FROM branch_card_status_current
						INNER JOIN branch
							ON branch_card_status_current.branch_id = branch.branch_id	
				WHERE branch_card_statuses_id = @branch_card_statuses_id
					AND product_id = COALESCE(@product_id, product_id)
					AND card_issue_method_id = @card_issue_method_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id
					AND card_id NOT IN(SELECT card_id 
										FROM [dist_batch_cards] 
											INNER JOIN [dist_batch]
												ON [dist_batch_cards].dist_batch_id = [dist_batch].dist_batch_id
												AND [dist_batch].dist_batch_type_id = 0
												AND [dist_batch].issuer_id = @issuer_id)


				--add prod batch status of created
				INSERT INTO [dbo].[dist_batch_status]
					([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@dist_batch_id, 0, @audit_user_id, SYSDATETIMEOFFSET(), 'Dist Batch Create')

				--Generate dist batch reference
				SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										  CONVERT(VARCHAR(MAX),[issuer_product].product_id) + '' +										  
										  CONVERT(VARCHAR(8), SYSDATETIMEOFFSET(), 112) + '' +
										  CAST(@dist_batch_id AS varchar(max))
				FROM [issuer]					
					INNER JOIN [issuer_product]
						ON [issuer_product].issuer_id = [issuer].issuer_id
				WHERE [issuer].issuer_id = @issuer_id

				SELECT @cards_in_batch = COUNT(*)
				FROM dist_batch_cards
				WHERE dist_batch_id = @dist_batch_id 

				--UPDATE prod batch with reference and number of cards
				UPDATE [dist_batch]
				SET [dist_batch_reference] = @dist_batch_ref,
					[no_cards] = @cards_in_batch
				WHERE [dist_batch].dist_batch_id = @dist_batch_id

				--update prod batch status to sent to cms
				UPDATE dist_batch_status 
					SET [dist_batch_statuses_id] = 10, 
						[user_id] = @audit_user_id, 
						[status_date] = DATEADD(ss, 1, SYSDATETIMEOFFSET()), 
						[status_notes] = ''	
				OUTPUT Deleted.* INTO dist_batch_status_audit
				WHERE [dist_batch_id] = @dist_batch_id

				--INSERT INTO [dbo].[dist_batch_status]
				--	([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				--VALUES(@dist_batch_id, 10, @audit_user_id, DATEADD(ss, 1, SYSDATETIMEOFFSET()), '')


				--UPDATE branch card status for those cards that have been added to the new dist batch.
				--INSERT INTO [branch_card_status]
				--	(branch_card_statuses_id, card_id, comments, status_date, [user_id])
				--SELECT 10, card_id, 'Assigned to batch', GETDATE(), @audit_user_id
				--FROM dist_batch_cards
				--WHERE dist_batch_id = @dist_batch_id	

				--Add audit for dist batch creation	
				DECLARE @dist_batch_status_name varchar(50)
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

				SELECT @ResultCode = 0
				COMMIT TRANSACTION [CREATE_CMS_BATCH]	

			END					
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [CREATE_CMS_BATCH]
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