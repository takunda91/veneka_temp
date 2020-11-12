-- =============================================
-- Author:		Selebalo, Setenane
-- Create date: 2014/03/20
-- Description:	Gets the issuer interface details based on the @interface_id
-- =============================================

CREATE PROCEDURE [dbo].[usp_get_issuer_interface_details]
	@connection_parameter_id int,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [connection_parameters]
	WHERE connection_parameter_id = @connection_parameter_id

END