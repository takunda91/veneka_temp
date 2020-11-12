-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_issue_reasons]
	-- Add the parameters for the stored procedure here
	@product_id int,
	@language_id int, 
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM [product_issue_reason]
	WHERE product_id = @product_id
END