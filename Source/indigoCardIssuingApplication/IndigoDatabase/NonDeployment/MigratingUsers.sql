

Create table #temp(ID INT identity(1,1) ,connect_parameter_id int,external_interface_id char(36))

insert into #temp(connect_parameter_id,external_interface_id)
select distinct connection_parameter_id,external_interface_id from dbo.[user]
select * from #temp
DECLARE @LoopCounter INT , @MaxEmployeeId INT, 
        @connect_parameter_id NVARCHAR(100),
        @external_interface_id NVARCHAR(100)
SELECT @LoopCounter = min(id) , @MaxEmployeeId = max(Id) 
FROM #temp
 Declare @authConfigId   INT;
WHILE(@LoopCounter IS NOT NULL
      AND @LoopCounter <= @MaxEmployeeId)
BEGIN
   SELECT @connect_parameter_id = connect_parameter_id,@external_interface_id=external_interface_id
   FROM #temp WHERE Id = @LoopCounter
   SET @authConfigId=0
   IF ( @connect_parameter_id is null and @external_interface_id is null)
   BEGIN
 

   INSERT INTO auth_configuration
                         (authentication_configuration)
	VALUES        ('Indigo User Auth')
	SET @authConfigId=SCOPE_IDENTITY()
	INSERT INTO auth_configuration_connection_parameters
                         (authentication_configuration_id, auth_type_id, connection_parameter_id, interface_guid, external_interface_id)
VALUES        (@authConfigId,NULL,@connect_parameter_id,NULL,@external_interface_id)
  	update dbo.[user] set authentication_configuration_id=@authConfigId where  connection_parameter_id is null   and external_interface_id is null


END
 ELSE

 BEGIN

 INSERT INTO auth_configuration
                         (authentication_configuration)
	VALUES        ('User Auth Config'+CAST(@LoopCounter as varchar))

	SET @authConfigId=SCOPE_IDENTITY()
	
	INSERT INTO auth_configuration_connection_parameters
                         (authentication_configuration_id, auth_type_id, connection_parameter_id, interface_guid, external_interface_id)
VALUES        (@authConfigId,0,@connect_parameter_id,NULL,@external_interface_id)
	
  	update dbo.[user] set authentication_configuration_id=@authConfigId where  connection_parameter_id =  @connect_parameter_id and external_interface_id = @external_interface_id

 END
    
   SET @LoopCounter  = @LoopCounter  + 1 
          
END
drop table #temp

