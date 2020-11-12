CREATE PROCEDURE [dbo].[usp_get_issuedcardsreport]
	@isuerid int,
	@fromdate datetimeoffset,
	@todate datetimeoffset,
	@userid int = null,
	@branchid int=null,
	@product_id int=null,
	@life_cycle int =null,
	@language_id int
	,@audit_user_id bigint
	,@audit_workstation varchar(100)

AS
BEGIN
if(@life_cycle=0)
	begin
	exec [usp_get_issuedcardsreport_issuedlifecycle] @isuerid,@fromdate,@todate,@userid,@branchid,@product_id,@language_id,@audit_user_id,@audit_workstation
	end
	else if(@life_cycle=1)
	begin
	 exec [usp_get_issuedcardsreport_fulllifecycle]  @isuerid,@fromdate,@todate,@userid,@branchid,@product_id,@language_id,@audit_user_id,@audit_workstation

	end
END