-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/25
-- Description:	Gets a list of issuer interfaces based on the @issuer_id
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_issuer_interfaces]
	@issuer_id int = NULL,
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
	FROM [issuer_interface]
	WHERE issuer_id = COALESCE(@issuer_id, issuer_id) AND
		  interface_type_id = COALESCE(@interface_type_id, interface_type_id) AND
		  interface_area = COALESCE(@interface_area, interface_area)


END