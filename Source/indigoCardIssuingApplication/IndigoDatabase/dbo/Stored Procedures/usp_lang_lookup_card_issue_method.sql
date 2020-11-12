-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_card_issue_method]
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT        card_issue_method_language.language_text, card_issue_method_language.card_issue_method_id as lookup_id
  FROM            card_issue_method INNER JOIN
                         card_issue_method_language ON card_issue_method.card_issue_method_id = card_issue_method_language.card_issue_method_id
						 where card_issue_method_language.language_id=@language_id
END