Create PROCEDURE [dbo].[usp_lang_lookup_Threed_batch_statues]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;




SELECT         threed_batch_statuses_id AS lookup_id, language_text
FROM            threed_secure_batch_statuses_language
	WHERE language_id = @language_id
END