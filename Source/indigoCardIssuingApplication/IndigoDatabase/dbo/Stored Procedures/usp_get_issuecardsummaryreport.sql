CREATE PROCEDURE [dbo].[usp_get_issuecardsummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@product_id int=null,
	@life_cycle int =null,
	@fromdate datetimeoffset,
	@todate datetimeoffset,	
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	if(@life_cycle=0)
	begin
	exec [usp_get_issuecardsummaryreport_issuedlifecyle] @branch_id,@issuer_id,@language_id,@product_id,@fromdate,@todate,@audit_user_id,@audit_workstation
	end
	else if(@life_cycle=1)
	begin
	 exec [usp_get_issuecardsummaryreport_fulllifecyle] @branch_id,@issuer_id,@language_id,@product_id,@fromdate,@todate,@audit_user_id,@audit_workstation

	end
	

END
