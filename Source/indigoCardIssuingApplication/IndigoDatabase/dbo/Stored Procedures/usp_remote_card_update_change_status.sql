CREATE PROCEDURE [dbo].[usp_remote_card_update_change_status]
	@card_id bigint,
	@remote_card_update_statuses_id int,
	@comment nvarchar(max),
	@language_id int = 0,
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
	WHERE card_id = @card_id

	--INSERT INTO [dbo].[remote_update_status] (card_id, comments, remote_component, remote_update_statuses_id, remote_updated_time, [user_id], status_date)
	--VALUES (@card_id, @comment, 'INDIGO_SYSTEM', @remote_card_update_statuses_id, null, @audit_user_id, SYSDATETIMEOFFSET())

	EXEC [usp_remote_card_update_get_detail] @card_id, @language_id, @audit_user_id, @audit_workstation
