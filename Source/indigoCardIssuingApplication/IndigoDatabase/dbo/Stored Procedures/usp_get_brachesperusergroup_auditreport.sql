-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_get_brachesperusergroup_auditreport] 
@issuer_id int=null,
@user_group_id int =null,
@branch_id int =null,
@user_role_id int=null,
@audit_user_id bigint,
@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	if(@issuer_id=-1 or @issuer_id=0)
	set @issuer_id=null

(SELECT  distinct      ug.user_group_name,issuer.issuer_code+'-'+ issuer.issuer_name as 'issuer_name', ug.mask_screen_pan, ug.mask_report_pan, ug.can_read, ug.can_update, ug.can_delete, ug.all_branch_access, 
                         case when all_branch_access=1 then 'ALL' else branch.branch_code+'-'+ branch.branch_name end as 'branch_name', '' as user_role, '' as username, ug.can_create,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
FROM            user_group AS ug left join  user_groups_branches on
						ug.user_group_id = user_groups_branches.user_group_id
						left join branch on branch.branch_id=user_groups_branches.branch_id
						inner join
                         issuer ON ug.issuer_id = issuer.issuer_id 
						 INNER JOIN
                         user_roles ON ug.user_role_id = user_roles.user_role_id
	 WHERE ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
			and ug.user_group_id =COALESCE(@user_group_id, ug.user_group_id)
			--and branch.branch_id=COALESCE(@branch_id, branch.branch_id)
				and ug.user_role_id=COALESCE(@user_role_id, ug.user_role_id) and all_branch_access=1
				)
	union
	
	(SELECT  distinct      ug.user_group_name, issuer.issuer_code+'-'+ issuer.issuer_name as 'issuer_name', ug.mask_screen_pan, ug.mask_report_pan, ug.can_read, ug.can_update, ug.can_delete, ug.all_branch_access, 
                         case when all_branch_access=1 then 'ALL' else branch.branch_code+'-'+ branch.branch_name end as 'branch_name', '' as user_role, '' as username, ug.can_create,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
FROM            user_group AS ug 
						inner join  user_groups_branches on
						ug.user_group_id = user_groups_branches.user_group_id
						inner join branch on branch.branch_id=user_groups_branches.branch_id
						inner join
                         issuer ON ug.issuer_id = issuer.issuer_id 
						 INNER JOIN
                         user_roles ON ug.user_role_id = user_roles.user_role_id
						 
	 WHERE ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
			and ug.user_group_id =COALESCE(@user_group_id, ug.user_group_id)
			and branch.branch_id=COALESCE(@branch_id, branch.branch_id)
				and ug.user_role_id=COALESCE(@user_role_id, ug.user_role_id)
	)

	--ORDER BY 	issuer.issuer_name,ug.user_group_name, branch_name
  
END