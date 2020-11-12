-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec usp_get_userbyroles_auditreport -1 ,null,null,null
CREATE PROCEDURE [dbo].[usp_get_userbyroles_auditreport]
@issuer_id int=null,
@user_group_id int =null,
@user_role_id int =null,
--@user_id int=null,
@audit_user_id bigint,
@audit_workstation varchar(100)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		OPEN SYMMETRIC KEY Indigo_Symmetric_Key
	DECRYPTION BY CERTIFICATE Indigo_Certificate;
	if(@issuer_id=-1 or @issuer_id=0)
	set @issuer_id=null

	DECLARE @UserTimezone as nvarchar(50)=[dbo].[GetUserTimeZone](@audit_user_id);

	SELECT        ug.user_group_name, user_roles.user_role, issuer.issuer_code+'-'+ issuer.issuer_name as 'issuer_name', ug.mask_screen_pan, ug.mask_report_pan, ug.can_read, ug.can_update, ug.can_delete, 
                         ug.all_branch_access, CONVERT(VARCHAR(max),DECRYPTBYKEY([user].username)) as 'username','' as branch_name, ug.can_create,CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(),@UserTimezone) as datetime) as 'excuted_datetime'
FROM            user_group AS ug INNER JOIN
                         issuer ON ug.issuer_id = issuer.issuer_id INNER JOIN
                         user_roles ON ug.user_role_id = user_roles.user_role_id INNER JOIN
                         users_to_users_groups ON ug.user_group_id = users_to_users_groups.user_group_id INNER JOIN
                         [user] ON users_to_users_groups.user_id = [user].user_id

	 WHERE ug.issuer_id = COALESCE(@issuer_id, ug.issuer_id)
			and ug.user_group_id =COALESCE(@user_group_id, ug.user_group_id)
			and ug.user_role_id=COALESCE(@user_role_id, ug.user_role_id)
			--and [user].user_id=COALESCE(@user_id, [user].user_id)
					
	ORDER BY 	issuer.issuer_name,ug.user_group_name,user_roles.user_role,username

  CLOSE SYMMETRIC KEY Indigo_Symmetric_Key;
END