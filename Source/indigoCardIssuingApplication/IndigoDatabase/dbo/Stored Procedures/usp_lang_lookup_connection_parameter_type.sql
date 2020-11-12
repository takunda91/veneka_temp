-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_lang_lookup_connection_parameter_type]
	@language_id int,
		@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  connection_parameter_type_id as lookup_id, language_text
	FROM connection_parameter_type_language 				
	WHERE language_id = @language_id
END