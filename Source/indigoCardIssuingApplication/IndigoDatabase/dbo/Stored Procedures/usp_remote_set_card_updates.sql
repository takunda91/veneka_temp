CREATE PROCEDURE [dbo].[usp_remote_set_card_updates]
	@remote_component_address varchar(250),
	@card_updates AS [remote_card_updates] READONLY,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

        BEGIN TRANSACTION [REMOTE_SET_CARD_UPDATES]
		BEGIN TRY 
			
			DECLARE @status_date datetime2 = SYSDATETIMEOFFSET()

			--Insert successful updates
			UPDATE t
				SET t.comments = cards.[comment], 
				t.remote_component = @remote_component_address, 
				t.remote_update_statuses_id = 2, 
				t.remote_updated_time = [cards].time_update, 
				t.[user_id] = @audit_user_id, 
				t.status_date = @status_date
			OUTPUT Deleted.* INTO [dbo].[remote_update_status_audit]
			FROM [dbo].[remote_update_status] AS t 
					INNER JOIN @card_updates AS cards ON t.[card_id] = cards.[card_id]
			WHERE cards.[successful] = 1

			--INSERT INTO [dbo].[remote_update_status] (card_id, comments, remote_component, remote_update_statuses_id, status_date, [remote_updated_time], [user_id])
			--SELECT [card_id], [comment], @remote_component_address, 2, @status_date, time_update, @audit_user_id
			--FROM @card_updates
			--WHERE successful = 1

			--Insert unsuccessful updates
			UPDATE t
				SET t.comments = cards.[comment], 
				t.remote_component = @remote_component_address, 
				t.remote_update_statuses_id = 4, 
				t.remote_updated_time = [cards].time_update, 
				t.[user_id] = @audit_user_id, 
				t.status_date = @status_date
			OUTPUT Deleted.* INTO [dbo].[remote_update_status_audit]
			FROM [dbo].[remote_update_status] AS t 
					INNER JOIN @card_updates AS cards ON t.[card_id] = cards.[card_id]
			WHERE cards.[successful] = 0

			--INSERT INTO [dbo].[remote_update_status] (card_id, comments, remote_component, remote_update_statuses_id, status_date, [remote_updated_time], [user_id])
			--SELECT [card_id], [comment], @remote_component_address, 4, @status_date, time_update, @audit_user_id
			--FROM @card_updates
			--WHERE successful = 0


			COMMIT TRANSACTION [REMOTE_SET_CARD_UPDATES]
		END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION [REMOTE_SET_CARD_UPDATES]
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
