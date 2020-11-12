CREATE PROCEDURE [dbo].[usp_get_Spoilcardsummaryreport]
	@branch_id int = null,
	@issuer_id int = null,
	@language_id int,
	@fromdate datetimeoffset,
	@todate datetimeoffset,	
	@product_id int=null,
	@life_cycle int=null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)

	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
   if(@life_cycle=0)
	begin
	exec  [usp_get_Spoilcardsummaryreport_issuedlifecycle] @branch_id,@issuer_id,@language_id,@fromdate,@todate,@product_id,@audit_user_id,@audit_workstation
	end
	else if(@life_cycle=1)
	begin
	 exec [usp_get_Spoilcardsummaryreport_fulllifecycle]  @branch_id,@issuer_id,@language_id,@fromdate,@todate,@product_id,@audit_user_id,@audit_workstation

	end
	else if(@life_cycle=2)
	begin
	 exec [usp_get_Spoilcardsummaryreport_postissuedlifecycle]  @branch_id,@issuer_id,@language_id,@fromdate,@todate,@product_id,@audit_user_id,@audit_workstation

	end
END


