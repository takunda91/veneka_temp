
-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Stored procedure for language lookup
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_customer_account_types] 
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
	SELECT [customer_account_type].account_type_id AS lookup_id, language_text
	FROM [customer_account_type_language]
		INNER JOIN [customer_account_type]
			ON [customer_account_type_language].account_type_id = [customer_account_type].account_type_id
	WHERE language_id = @language_id
		AND active_YN = 1

END