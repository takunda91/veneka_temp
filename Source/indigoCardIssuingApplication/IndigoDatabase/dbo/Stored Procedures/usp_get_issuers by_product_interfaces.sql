-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	Gets a list of issuer id's based on the interface
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuers by_product_interfaces]
	@interface_type_id int = NULL,
	@connection_parameter_id int = NULL,
	@interface_guid varchar(36) = NULL,	
	@interface_area int = NULL,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT issuer_id
	FROM [dbo].[product_interface] INNER JOIN [dbo].[issuer_product]
		ON [dbo].[issuer_product].product_id = [dbo].[product_interface].product_id
	WHERE [dbo].[product_interface].interface_type_id = COALESCE(@interface_type_id, interface_type_id)
		AND [dbo].[product_interface].connection_parameter_id = COALESCE(@connection_parameter_id, connection_parameter_id) 
		AND [dbo].[product_interface].interface_guid = COALESCE(@interface_guid, interface_guid) 
		AND [dbo].[product_interface].interface_area = COALESCE(@interface_area, interface_area)

END