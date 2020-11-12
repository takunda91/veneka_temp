EXEC sp_RENAME 'dbo.[user].ldap_setting_id' , 'connection_parameter_id', 'COLUMN'

GO

alter table dbo.[user]
add external_interface_id char(36)
GO

  ALTER TABLE dbo.[user]
   add foreign key (connection_parameter_id) references connection_parameters  (connection_parameter_id)
GO






