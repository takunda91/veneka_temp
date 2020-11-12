-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_create_export_batches]
	-- Add the parameters for the stored procedure here
	@issuer_id int = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @product_id int, @issuer_id_cursor int	
	DECLARE @confirmed_export_batches TABLE (export_batch_id bigint)
	DECLARE @export_cards TABLE (card_id bigint)
	DECLARE @export_batch TABLE (ID bigint)
	DECLARE @product_code varchar(50)

	SET @ResultCode = 0

	--Find products who have exportable set to true and are not deleted.
	DECLARE export_product_cursor CURSOR FOR 
		SELECT product_id, issuer_id, product_code 
		FROM [issuer_product] 
		WHERE cms_exportable_YN = 1 
				AND DeletedYN = 0
				AND issuer_id = COALESCE(@issuer_id, issuer_id)
	OPEN export_product_cursor
	FETCH NEXT FROM export_product_cursor
	INTO @product_id, @issuer_id_cursor, @product_code

		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			DECLARE @create_export_batch_statuses_id int = 0
				
			DELETE FROM @export_cards

			INSERT INTO @export_cards (card_id)
			SELECT [cards].card_id 
			FROM [cards] INNER JOIN [branch_card_status_current]						
					ON [cards].card_id = [branch_card_status_current].card_id 					
			WHERE [cards].product_id = @product_id 
				AND [branch_card_status_current].branch_card_statuses_id = 6
				AND [cards].export_batch_id IS NULL
				
			--Are there any cards that need a batch created
			IF EXISTS(SELECT * FROM @export_cards)
				BEGIN
					DECLARE @export_batch_id bigint

					BEGIN TRANSACTION [CREATE_EXPORT_BATCHES]
					BEGIN TRY 
						
						DECLARE @no_cards int						
						DELETE FROM @export_batch

						SELECT @no_cards = COUNT(card_id) FROM @export_cards

						--CREATE NEW EXPORT BATCH
						INSERT INTO [export_batch] (batch_reference, date_created, issuer_id, no_cards)
							OUTPUT inserted.export_batch_id INTO @export_batch(ID)
						VALUES (@product_code + '_' + CAST(SYSDATETIME() as varchar(max)), SYSDATETIMEOFFSET(), @issuer_id_cursor, @no_cards)

						--DECLARE @export_batch_id bigint
						SELECT TOP 1 @export_batch_id = ID FROM @export_batch

						--ADD CREATED STATUS
						INSERT INTO [export_batch_status] (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
						VALUES (@export_batch_id, @create_export_batch_statuses_id, SYSDATETIMEOFFSET(), @audit_user_id, 'CREATED')
						
						--LINK CARDS TO EXPORT BATCH
						UPDATE [cards]
						SET export_batch_id = @export_batch_id
						WHERE card_id IN (SELECT card_id FROM @export_cards)
						
						--AUDIT CREATE
						DECLARE @batch_status_name varchar(100),
								@batch_ref varchar(100),
								@audit_msg varchar(max)

						SELECT @batch_status_name =  export_batch_statuses_name
						FROM export_batch_statuses
						WHERE export_batch_statuses_id = @create_export_batch_statuses_id

						SELECT @batch_ref = batch_reference
						FROM export_batch
						WHERE export_batch_id = @export_batch_id

						--Add audit for pin batch update								
						SET @audit_msg = 'Create: ' + CAST(@export_batch_id AS varchar(max)) +
											', ' + COALESCE(@batch_ref, 'UNKNOWN') +
											', ' + COALESCE(@batch_status_name, 'UNKNOWN')
								   
						--log the audit record		
						EXEC usp_insert_audit @audit_user_id, 
												11,
												NULL, 
												@audit_workstation, 
												@audit_msg, 
												NULL, NULL, NULL, NULL


						--AUTO-APPROVE BATCH?
						--DECLARE @ApproveResultCode int
						--DECLARE @hideResults TABLE (newvalue int)
						--INSERT INTO @hideResults 
						--EXEC usp_export_batch_status_approve @export_batch_id, 1, 0, @audit_user_id, @audit_workstation, @ApproveResultCode OUTPUT
						UPDATE [export_batch_status]
							SET [export_batch_statuses_id] = 1, 
								[status_date] = DATEADD(MILLISECOND, 1, SYSDATETIMEOFFSET()), 
								[user_id] = @audit_user_id, 
								[comments] = 'AUTO APPROVED'
						OUTPUT Deleted.* INTO [export_batch_status_audit]
						WHERE [export_batch_id] = @export_batch_id

						--INSERT INTO [export_batch_status] (export_batch_id, export_batch_statuses_id, status_date, [user_id], comments)
						--VALUES (@export_batch_id, 1, DATEADD(MILLISECOND, 1, SYSDATETIMEOFFSET()), @audit_user_id, 'AUTO APPROVED')


						--CHECK THAT ALL CARDS HAVE BEEN LINKED, if it doesnt match rollback for this product
						IF ( --@ApproveResultCode = 0 AND
								(SELECT COUNT(*) FROM @export_cards) =
								(SELECT COUNT(*) FROM [cards] WHERE export_batch_id = @export_batch_id
									AND card_id IN (SELECT card_id FROM @export_cards)))
							BEGIN
								INSERT INTO @confirmed_export_batches(export_batch_id)
								VALUES (@export_batch_id)

								COMMIT TRANSACTION [CREATE_EXPORT_BATCHES]
							END
						ELSE
							BEGIN
								SET @ResultCode = 1000				
								ROLLBACK TRANSACTION [CREATE_EXPORT_BATCHES]
							END

					END TRY
					BEGIN CATCH
						SET @ResultCode = 1000	
						ROLLBACK TRANSACTION [CREATE_EXPORT_BATCHES]
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
		
			FETCH NEXT FROM export_product_cursor
			INTO @product_id, @issuer_id_cursor, @product_code
		END 
	CLOSE export_product_cursor;
	DEALLOCATE export_product_cursor;

	SELECT * FROM [export_batch]
	WHERE export_batch_id IN (SELECT export_batch_id FROM @confirmed_export_batches)
END