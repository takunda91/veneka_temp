-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_lookup_response_message] 
	-- Add the parameters for the stored procedure here
	@system_response_code int,
	@system_area int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT english_response, french_response, portuguese_response, spanish_response
	FROM [response_messages]
	WHERE system_response_code = @system_response_code
		  AND system_area = @system_area

END