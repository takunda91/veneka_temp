-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_pinreissue_report]
	@isuerid int,
	@fromdate DATETIMEOFFSET,
	@todate DATETIMEOFFSET,
	@userid int = null,
	@product_id int=null,
	@branchid int=null,
	@language_id int = null,
	@audit_user_id bigint,
	@audit_workstation varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@userid=0)
	set @userid=null

	if(@isuerid=0)
	set @isuerid=null

	
	if(@branchid=0)
	set @branchid=null

	SET @todate = DATEADD(d, 1, @todate)
		DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	DECLARE @mask_report bit = [dbo].MaskReportPAN(@audit_user_id)
	

    -- Insert statements for procedure here
	OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;

		 SELECT branch.branch_code+'-'+branch.branch_name as 'branchcode', 
			issuer.issuer_code+'-'+	issuer.issuer_name as 'issuer_name', 
				pin_reissue.pan, 
				CONVERT(VARCHAR,DECRYPTBYKEY([user].username)) as 'IssuedBy',  
				CONVERT(VARCHAR,DECRYPTBYKEY(apporved.username)) as 'APPROVER_USER', 
				CASE 
					WHEN pin_reissue_status_current.pin_reissue_statuses_id = 2 
					THEN CAST(SWITCHOFFSET(pin_reissue_status_current.status_date,@UserTimezone) as datetime)   
					ELSE  CAST(SWITCHOFFSET(pin_reissue.reissue_date,@UserTimezone) as datetime) 
				END AS 'approveddate',
				CASE 
					WHEN @mask_report = 1 
					THEN [dbo].[MaskString](CONVERT(VARCHAR(MAX),DECRYPTBYKEY(pin_reissue.pan)),6,4) 
					ELSE CONVERT(VARCHAR(MAX),DECRYPTBYKEY(pin_reissue.pan))
				END AS 'cardnumber', 
				'pin re-issue' as Reason,
				CAST(SWITCHOFFSET(pin_reissue.reissue_date,@UserTimezone) as datetime)  as 'ReIssuedDate'
				,issuer_product.product_code+'-'+issuer_product.product_name as product
				,issuer_product.product_id
			FROM pin_reissue 
					LEFT JOIN pin_reissue_status_current 
						ON pin_reissue.pin_reissue_id = pin_reissue_status_current.pin_reissue_id
                    INNER JOIN  branch 
						ON pin_reissue.branch_id = branch.branch_id 
					INNER JOIN issuer 
						ON pin_reissue.issuer_id = issuer.issuer_id 
					INNER JOIN [user] 
						ON pin_reissue.operator_user_id = [user].[user_id] 
					LEFT JOIN [user] as apporved 
						ON pin_reissue.authorise_user_id = apporved.[user_id] 
					INNER JOIN [issuer_product] 
						ON pin_reissue.product_id=[issuer_product].product_id
			WHERE 
				issuer.issuer_id=COALESCE(@isuerid, issuer.issuer_id)AND
				branch.branch_id = COALESCE(@branchid, branch.branch_id)
				AND CAST(SWITCHOFFSET( pin_reissue.reissue_date,@UserTimezone) as datetime)  >= @fromdate 
				AND CAST(SWITCHOFFSET( pin_reissue.reissue_date,@UserTimezone) as datetime) <= @todate 
				AND (pin_reissue_status_current.pin_reissue_statuses_id=3 or pin_reissue_status_current.pin_reissue_statuses_id=4)
				AND pin_reissue.product_id=COALESCE(@product_id, pin_reissue.product_id)

   CLOSE SYMMETRIC KEY Indigo_Symmetric_Key


END