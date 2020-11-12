-- =============================================
-- Author:		Richard Brenchley
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_product_fee_details]
	@fee_scheme_id int	,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @UserTimezone nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

 SELECT        fee_scheme_id, fee_detail_id, fee_detail_name,CAST(SWITCHOFFSET( effective_from,@UserTimezone) AS datetime) as effective_from, fee_waiver_YN, fee_editable_YN, deleted_yn, CAST(SWITCHOFFSET( effective_to,@UserTimezone) AS datetime) as effective_to
FROM            product_fee_detail
	WHERE fee_scheme_id = @fee_scheme_id
END