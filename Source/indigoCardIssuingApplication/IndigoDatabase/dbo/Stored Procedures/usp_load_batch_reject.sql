-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Reject the load batch
-- =============================================
CREATE PROCEDURE [dbo].[usp_load_batch_reject] 
	@load_batch_id bigint,
	@status_notes varchar(150),
	@audit_user_id bigint,
	@audit_workstation varchar(100),
	@ResultCode int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION [REJECT_LOAD_BATCH]
		BEGIN TRY 
			
			DECLARE @current_status int

			SELECT @current_status = load_batch_statuses_id
			FROM load_batch_status_current
			WHERE load_batch_id = @load_batch_id
										  
			--Check that someone hasn't already updated the load batch
			IF(@current_status <> 0)
				BEGIN
					SET @ResultCode = 100
				END
			ELSE
				BEGIN
					--Update the load batch status.
					UPDATE load_batch_status
						SET [load_batch_statuses_id] = 2,
							[user_id] = @audit_user_id, 
							[status_date] = SYSDATETIMEOFFSET(), 
							[status_notes] = @status_notes
					OUTPUT Deleted.* INTO load_batch_status_audit
					WHERE [load_batch_id] = @load_batch_id

					--INSERT load_batch_status
					--		([load_batch_id], [load_batch_statuses_id], [user_id], [status_date], [status_notes])
					--VALUES (@load_batch_id, 2, @audit_user_id, SYSDATETIMEOFFSET(), @status_notes)

					--Update the cards linked to the load batch with the new status.
					UPDATE load_batch_cards
					SET load_card_status_id = 3
					WHERE load_batch_id = @load_batch_id


					DECLARE @product_load_type_id int

					--Get the load type for the product
					SELECT @product_load_type_id = product_load_type_id
					FROM [issuer_product] 
					WHERE product_id = (SELECT TOP 1 product_id 
										FROM [cards] INNER JOIN [load_batch_cards]
											ON [load_batch_cards].card_id = [cards].card_id
										WHERE [load_batch_cards].load_batch_id = @load_batch_id)

					IF(@product_load_type_id = 4)
						BEGIN							
							UPDATE [dist_batch_status]
								SET [dist_batch_statuses_id] = 20, 
									[user_id] = @audit_user_id, 
									[status_date] = SYSDATETIMEOFFSET(), 
									[status_notes] = 'Load for order rejected.'	
							OUTPUT Deleted.* INTO dist_batch_status_audit
							WHERE [dist_batch_id] IN (
								SELECT DISTINCT dbc.dist_batch_id
								FROM [dist_batch] dbc
									INNER JOIN [dist_batch_cards]
										ON [dist_batch_cards].dist_batch_id = dbc.dist_batch_id
									INNER JOIN [load_batch_cards]
										ON [dist_batch_cards].card_id = [load_batch_cards].card_id
								WHERE 20 = ALL(SELECT dist_card_status_id 
												FROM [dist_batch_cards] dbc2 						
												WHERE dbc2.dist_batch_id = dbc.dist_batch_id)
										AND [load_batch_cards].load_batch_id = @load_batch_id
							)
						END	
	

					--log the audit record for load batch
					DECLARE @audit_msg varchar(max),
							@load_batch_ref varchar(100),
							@load_batch_status varchar(50)

					SELECT @load_batch_ref = load_batch_reference
					FROM [load_batch]
					WHERE load_batch_id = @load_batch_id

					SELECT @load_batch_status = load_batch_status_name
					FROM [load_batch_statuses]
					WHERE load_batch_statuses_id = 3
						
					SET @audit_msg = 'Update: ' + CAST(@load_batch_id AS varchar(max)) + 
									 ', ' + COALESCE(@load_batch_ref, 'UNKNWON') +
									 ', ' + COALESCE(@load_batch_status, 'UNKNOWN')

					--log the audit record		
					EXEC usp_insert_audit @audit_user_id, 
										 5,---LoadBatch
										 NULL, 
										 @audit_workstation, 
										 @audit_msg, 
										 NULL, NULL, NULL, NULL

					SET @ResultCode = 0

					COMMIT TRANSACTION [REJECT_LOAD_BATCH]
				END
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REJECT_LOAD_BATCH]
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

