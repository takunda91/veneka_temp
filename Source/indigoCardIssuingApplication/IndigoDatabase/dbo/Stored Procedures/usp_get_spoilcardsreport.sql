CREATE PROCEDURE [dbo].[usp_get_spoilcardsreport]
	@isuerid int = null,
	@language_id int,
	@userid int = null,
	@branchid int = null,
	@fromdate datetimeoffset,
	@todate datetimeoffset,
	@life_cycle int=null,
	@product_id int=null,
	@audit_user_id BIGINT,
	@audit_workstation VARCHAR(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if(@life_cycle=0)
	begin
	exec [usp_get_spoilcardsreport_issuedlifecycle] @isuerid,@language_id,@userid,@branchid,@fromdate,@todate,@product_id,@audit_user_id,@audit_workstation
	end
	else if(@life_cycle=1)
	begin
	 exec [usp_get_spoilcardsreport_fulllifecycle]  @isuerid,@language_id,@userid,@branchid,@fromdate,@todate,@product_id,@audit_user_id,@audit_workstation

	end

END