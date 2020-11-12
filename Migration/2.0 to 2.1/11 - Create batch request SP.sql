-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE sp_request_create_dist_batch 
	@card_issue_method_id int,
	@branch_id int,
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


		--Only create a batch if there are cards for the batch
		IF( (SELECT COUNT(*) FROM branch_card_status_current
				WHERE branch_card_statuses_id = 3 
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND card_priority_id = @card_priority_id
					and branch_id = @branch_id) = 0)
		BEGIN
			SET @ResultCode = 400
			COMMIT TRANSACTION [CREATE_DIST_BATCH]
		END			
		ELSE
			BEGIN

				DECLARE @cards_total int = 0,
						@audit_msg varchar


				--create the distribution batch
				INSERT INTO [dist_batch]
					([card_issue_method_id],[branch_id], [no_cards],[date_created],[dist_batch_reference])
				VALUES (@card_issue_method_id, @branch_id, 0, GETDATE(), GETDATE())

				SET @dist_batch_id = SCOPE_IDENTITY();

				--Add cards to distribution batch
				INSERT INTO [dist_batch_cards]
					([dist_batch_id],[card_id],[dist_card_status_id])
				SELECT @dist_batch_id, card_id, 0
				FROM branch_card_status_current
				WHERE branch_card_statuses_id = 3 
					AND product_id = @product_id
					AND card_issue_method_id = @card_issue_method_id
					AND card_priority_id = @card_priority_id
					and branch_id = @branch_id


				--add dist batch status of created
				INSERT INTO [dbo].[dist_batch_status]
					([dist_batch_id],[dist_batch_statuses_id],[user_id],[status_date],[status_notes])
				VALUES(@dist_batch_id, 0, @audit_user_id, GETDATE(), 'Dist Batch Create')

				--Generate dist batch reference
				SELECT @dist_batch_ref =  [issuer].issuer_code + '' + 
										  [branch].branch_code + '' + 
										  CONVERT(VARCHAR(8), GETDATE(), 112) + '' +
										  CAST(@dist_batch_id AS varchar(max))
				FROM [branch] INNER JOIN [issuer]
					ON [branch].issuer_id = [issuer].issuer_id
				WHERE [branch].branch_id = @branch_id

				SELECT @cards_in_batch = COUNT(*)
				FROM dist_batch_cards
				WHERE dist_batch_id = @dist_batch_id 

				--UPDATE dist batch with reference and number of cards
				UPDATE [dist_batch]
				SET [dist_batch_reference] = @dist_batch_ref,
					[no_cards] = @cards_in_batch
				WHERE [dist_batch].dist_batch_id = @dist_batch_id


				--UPDATE branch card status for those cards that have been added to the new dist batch.
				INSERT INTO [branch_card_status]
					(branch_card_statuses_id, card_id, comments, status_date, [user_id])
				SELECT 10, card_id, 'Assigned to dist batch', GETDATE(), @audit_user_id
				FROM dist_batch_cards
				WHERE dist_batch_id = @dist_batch_id	

				--Add audit for dist batch creation	
				DECLARE @dist_batch_status_name varchar(50)
				SELECT @dist_batch_status_name =  dist_batch_status_name
				FROM dist_batch_statuses
				WHERE dist_batch_statuses_id = 0
											
				SET @audit_msg = 'Create: ' + CAST(@dist_batch_id AS varchar(max)) +
									', ' + COALESCE(@dist_batch_ref, 'UNKNOWN') +
									', ' + COALESCE(@dist_batch_status_name, 'UNKNOWN')
								   
				--log the audit record		
				EXEC sp_insert_audit @audit_user_id, 
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
GO
