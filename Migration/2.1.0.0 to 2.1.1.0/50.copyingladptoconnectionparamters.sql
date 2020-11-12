INSERT INTO connection_parameters
                         (connection_name, address, port, path, protocol, auth_type, username, password, connection_parameter_type_id, domain_name,file_encryption_type_id)
SELECT        ldap_setting_name, hostname_or_ip,0 as port, path,0 as protocol,0 as auth_type,  username, password,4 as connection_parameter_type_id,domain_name,1 as file_encryption_type_id
FROM            ldap_setting



Go

--- to upadte old data
declare  @temp table(ldap_setting_id int , connection_parameter_id int)
insert @temp(ldap_setting_id,connection_parameter_id)
select ldap_setting_id,c.connection_parameter_id
from ldap_setting
inner join connection_parameters c
on ldap_setting.ldap_setting_name =c.connection_name


update u set connection_parameter_id = t.connection_parameter_id
from dbo.[user] u inner join 
@temp t on  t.ldap_setting_id = u.connection_parameter_id