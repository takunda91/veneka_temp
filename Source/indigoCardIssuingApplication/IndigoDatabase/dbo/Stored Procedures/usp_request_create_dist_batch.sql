-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_request_create_dist_batch] 
	@card_issue_method_id int,
	@issuer_id int,
	@branch_id int = null,
	@product_id int,
	@card_priority_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@cards_in_batch int OUTPUT,
	@dist_batch_id int OUTPUT,
	@dist_batch_ref varchar(50) OUTPUT,
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION [CREATE_DIST_BATCH]
		BEGIN TRY 

		SET @cards_in_batch = 0
		SET	@dist_batch_id = 0
		SET @dist_batch_ref = ''

		DECLARE @branch_card_statuses_id int
		SET @branch_card_statuses_id = 3

		--RAB: Card should always be in approved for issue state. Card Request will do an "Auto Approval" for maker/checker
		--See if the issuer of the branch requires MakerChecker, set branch card statis accordingly.
		--IF((SELECT TOP 1 [issuer].maker_checker_YN
		 
		--    FROM [issuer] INNER JOIN [branch] ON [branch].issuer_id = [issuer].issuer_id 
		--    WHERE [branch].branch_id = @branch_id) = 1)
		--	BEGIN
		--		SET @branch_card_statuses_id = 3
		--	END
		--ELSE
		--	BEGIN
		--		SET @branch_card_statuses_id = 2
		--	END


		--Only create a batch if there are cards for the batch
		IF( (SELECT COUNT(*) 
			 FROM branch_card_status_current
					INNER JOIN branch
						ON branch_card_status_current.branch_id = branch.branch_id					
			 WHERE branch_card_statuses_id = @branch_card_statuses_id
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND card_priority_id = @card_priority_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id) = 0)
		BEGIN
			SET @ResultCode = 400
			COMMIT TRANSACTION [CREATE_DIST_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@batch_branch_id int,
						@audit_msg nvarchar(500)


				
				--SELECT TOP 1 @batch_branch_id = branch_card_status_current.branch_id
				--FROM branch_card_status_current
				--INNER JOIN branch
				--	ON branch_card_status_current.branch_id = branch.branch_id					
				--	WHERE branch_card_statuses_id = @branch_card_statuses_id
				--		AND product_id = @product_id
				--		AND card_issue_method_id = @card_issue_method_id
				--		AND card_priority_id = @card_priority_id
				--		AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
				--		AND issuer_id = @issuer_id


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
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND card_priority_id = @card_priority_id
					AND branch_card_status_current.branch_id = COALESCE(@branch_id, branch_card_status_current.branch_id)
					AND issuer_id = @issuer_id


				--add prod batch status of created
				INSERT INTO [dbo].[dist_batch_status]
					([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@dist_batch_id, 0, @audit_user_id, SYSDATETIMEOFFSET(), 'Batch Created')

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

				--UPDATE branch card status for those cards that have been added to the new dist batch.
				UPDATE t
				SET t.branch_id = [cards].branch_id, 
					t.branch_card_statuses_id = 10, 
					t.status_date = DATEADD(s, 1,SYSDATETIMEOFFSET()), 
					t.[user_id] = @audit_user_id, 
					t.operator_user_id = NULL,
					t.branch_card_code_id = NULL,
					t.comments = 'Assigned to batch',
					t.pin_auth_user_id = NULL
				OUTPUT Deleted.* INTO branch_card_status_audit
				FROM branch_card_status t 
						INNER JOIN [dist_batch_cards] s ON t.card_id = s.card_id
						INNER JOIN [cards] ON [cards].card_id = s.card_id
				WHERE s.dist_batch_id = @dist_batch_id

				--INSERT INTO [branch_card_status]
				--	(branch_card_statuses_id, card_id, branch_id, comments, status_date, [user_id])
				--SELECT 10, dist_batch_cards.card_id, [cards].branch_id, 'Assigned to batch', SYSDATETIMEOFFSET(), @audit_user_id
				--FROM dist_batch_cards INNER JOIN [cards]
				--	ON [cards].card_id = dist_batch_cards.card_id
				--WHERE dist_batch_id = @dist_batch_id

				--Notification
				EXEC usp_notification_batch_add @dist_batch_id, 0

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