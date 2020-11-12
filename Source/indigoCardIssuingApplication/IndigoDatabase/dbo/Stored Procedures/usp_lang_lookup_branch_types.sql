﻿CREATE PROCEDURE [dbo].[usp_lang_lookup_branch_types] 
	-- Add the parameters for the stored procedure here
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT branch_type_id AS lookup_id, language_text
	FROM branch_type_language
	WHERE language_id = @language_id
END
