-- =============================================
-- Author:		Richard Brenchley
-- Create date: 2015/07/28
-- Description:	Gets a list of product interfaces based on the @product_id
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_interfaces]
	@product_id int = NULL,
	@interface_type_id int = NULL,
	@interface_area int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	SELECT *
	FROM [product_interface]
	WHERE product_id = COALESCE(@product_id, product_id) AND
		  interface_type_id = COALESCE(@interface_type_id, interface_type_id) AND
		  interface_area = COALESCE(@interface_area, interface_area)


END