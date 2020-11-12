CREATE PROCEDURE [dbo].[usp_lang_lookup_hybrid_request_statues]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



SELECT         [hybrid_request_statuses_id] AS lookup_id, language_text
FROM            hybrid_request_statuses_language
	WHERE language_id = @language_id

END