CREATE PROCEDURE [dbo].[usp_remote_card_update_change_statuses]
	@card_id_list as [dbo].[card_id_array] READONLY,
	@remote_card_update_statuses_id int,
	@comment nvarchar(max),
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [dbo].[remote_update_status]
		SET comments = @comment, 
		remote_component = 'INDIGO_SYSTEM', 
		remote_update_statuses_id = @remote_card_update_statuses_id, 
		remote_updated_time = NULL, 
		[user_id] = @audit_user_id, 
		status_date = SYSDATETIMEOFFSET()
	OUTPUT Deleted.* INTO [dbo].[remote_update_status_audit]
	WHERE card_id IN (SELECT card_id FROM @card_id_list)

	--INSERT INTO [dbo].[remote_update_status] (card_id, comments, remote_component, remote_update_statuses_id, remote_updated_time, [user_id], status_date)
	--SELECT card_id, @comment, 'INDIGO_SYSTEM', @remote_card_update_statuses_id, null, @audit_user_id, SYSDATETIMEOFFSET()
	--FROM @card_id_list

