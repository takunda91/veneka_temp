CREATE PROCEDURE [dbo].[usp_lang_lookup_remote_update_statuses]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT remote_update_statuses_id lookup_id, language_text
	FROM [dbo].[remote_update_statuses_language]
	WHERE language_id = @language_id
END
