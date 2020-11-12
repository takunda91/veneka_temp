alter table connection_parameters
add domain_name varchar(100),
	is_external_auth bit 

	update connection_parameters set is_external_auth=0