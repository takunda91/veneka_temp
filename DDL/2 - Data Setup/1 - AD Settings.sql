USE [indigo_database_group]
GO

DECLARE @RC int
DECLARE @ldap_setting_name varchar(100) = 'AD_Settings'
DECLARE @hostname_or_ip varchar(200) = 'DC-01.example.group'
DECLARE @path varchar(200) = 'OU=Usr,DC=example,DC=group'
DECLARE @domain_name varchar(100) = 'ecobankgroup'
DECLARE @audit_user_id bigint = -2
DECLARE @audit_workstation varchar(100) = 'SYSTEM'
DECLARE @new_ldap_id int
DECLARE @ResultCode int

-- TODO: Set parameter values here.

EXECUTE @RC = [sp_create_ldap] 
   @ldap_setting_name
  ,@hostname_or_ip
  ,@path
  ,@domain_name
  ,NULL
  ,NULL
  ,@audit_user_id
  ,@audit_workstation
  ,@new_ldap_id OUTPUT
  ,@ResultCode OUTPUT
GO


