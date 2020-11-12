﻿
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_branch_statuses] 
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
	SELECT branch_status_id AS lookup_id, language_text
	FROM [branch_statuses_language]
	WHERE language_id = @language_id
	order by language_text
END